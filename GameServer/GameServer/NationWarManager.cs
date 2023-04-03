using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarManager
{
	public const int kNationWarPastDays = -7;

	private DateTime m_date = DateTime.MinValue.Date;

	private NationWar m_nationWar;

	private Dictionary<Guid, NationWarDeclaration> m_nationWarDeclarations = new Dictionary<Guid, NationWarDeclaration>();

	private Dictionary<Guid, NationWarDeclaration> m_pastNationWars = new Dictionary<Guid, NationWarDeclaration>();

	private Dictionary<Guid, NationWarInstance> m_nationWarInsts = new Dictionary<Guid, NationWarInstance>();

	private Dictionary<Guid, NationWarResult> m_nationWarResults = new Dictionary<Guid, NationWarResult>();

	private DateTimeOffset m_lastUpdateTime = DateTimeOffset.MinValue;

	public NationWarManager(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Init(DateTime date)
	{
		m_date = date;
	}

	public void OnUpdate(DateTimeOffset time)
	{
		Refresh(time.Date);
		OnUpdate_SystemNationWarDeclaration(time);
		OnUpdate_NationWarStart(time);
		OnUpdate_NationWarUpdate(time);
		m_lastUpdateTime = time;
	}

	private void OnUpdate_SystemNationWarDeclaration(DateTimeOffset time)
	{
		if (!m_nationWar.IsEnabledNationWarDeclaration(time))
		{
			return;
		}
		DateTime date = time.Date;
		DateTime serverOpenDate = Cache.instance.serverOpenDate;
		foreach (SystemNationWarDeclaration systemDeclaration in m_nationWar.systemDeclarations)
		{
			if (!(date != serverOpenDate.AddDays(systemDeclaration.serverOpenDayCount)) && !ContainsNationWarDeclarationByNationId(systemDeclaration.offenseNation.id) && !ContainsNationWarDeclarationByNationId(systemDeclaration.defenseNation.id))
			{
				NationWarDeclaration newDeclaration = new NationWarDeclaration();
				newDeclaration.Init(systemDeclaration.offenseNation, systemDeclaration.defenseNation, time);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(newDeclaration.nationId);
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(newDeclaration.targetNationId));
				dbWork.AddSqlCommand(GameDac.CSC_AddNationWarDeclaration(newDeclaration.id, newDeclaration.nationId, newDeclaration.targetNationId, 0, newDeclaration.regTime));
				dbWork.Schedule();
				try
				{
					SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
					logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarDeclarationLog(Guid.NewGuid(), 1, newDeclaration.id, newDeclaration.nationId, Guid.Empty, newDeclaration.targetNationId, 0, newDeclaration.regTime));
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
				}
				AddNationWarDeclaration(newDeclaration);
				ServerEvent.SendNationWarDeclaration(Cache.instance.GetClientPeers(Guid.Empty), newDeclaration.ToPDNationWarDeclaration());
			}
		}
	}

	private void OnUpdate_NationWarStart(DateTimeOffset time)
	{
		if (m_lastUpdateTime == DateTimeOffset.MinValue || m_nationWarDeclarations.Count == 0)
		{
			return;
		}
		DateTimeOffset startTime = time.Date.AddSeconds(m_nationWar.startTime);
		if (startTime <= m_lastUpdateTime || startTime > time)
		{
			return;
		}
		foreach (NationWarDeclaration nationWarDeclaration in m_nationWarDeclarations.Values)
		{
			NationWarInstance nationWarInst = new NationWarInstance();
			nationWarInst.Init(nationWarDeclaration, time);
			NationInstance offenseNationInst = Cache.instance.GetNationInstance(nationWarInst.offenseNation.id);
			offenseNationInst.ReadyNationWar(nationWarInst, time);
			NationInstance defenseNationInst = Cache.instance.GetNationInstance(nationWarInst.defenseNation.id);
			defenseNationInst.ReadyNationWar(nationWarInst, time);
			ServerEvent.SendNationWarStart(Cache.instance.GetClientPeers(Guid.Empty), nationWarDeclaration.id);
			nationWarInst.Start(time);
			m_nationWarInsts.Add(nationWarInst.declaration.id, nationWarInst);
		}
	}

	private void OnUpdate_NationWarUpdate(DateTimeOffset time)
	{
		NationWarInstance[] array = m_nationWarInsts.Values.ToArray();
		foreach (NationWarInstance nationWarInst in array)
		{
			nationWarInst.OnUpdate(time);
		}
	}

	public void Refresh(DateTime date)
	{
		if (!(m_date == date))
		{
			m_date = date;
			m_nationWarDeclarations.Clear();
		}
	}

	public void AddPastNationWar(NationWarDeclaration nationWarDeclaration)
	{
		if (nationWarDeclaration == null)
		{
			throw new ArgumentNullException("nationWarDeclaration");
		}
		m_pastNationWars.Add(nationWarDeclaration.id, nationWarDeclaration);
	}

	public void AddNationWarDeclaration(NationWarDeclaration nationWarDeclaration)
	{
		if (nationWarDeclaration == null)
		{
			throw new ArgumentNullException("nationWarDeclaration");
		}
		m_nationWarDeclarations.Add(nationWarDeclaration.id, nationWarDeclaration);
	}

	public bool ContainsNationWarDeclarationByNationId(int nNationId)
	{
		foreach (NationWarDeclaration declaration in m_nationWarDeclarations.Values)
		{
			if (declaration.nationId == nNationId || declaration.targetNationId == nNationId)
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsNationWarDeclarationByNationIdAndTargetNationId(int nNationId, int nTargetNationId)
	{
		foreach (NationWarDeclaration declaration in m_nationWarDeclarations.Values)
		{
			if (declaration.nationId == nNationId && declaration.targetNationId == nTargetNationId)
			{
				return true;
			}
		}
		return false;
	}

	public void OnNationWarInstanceFinish(NationWarResult result)
	{
		AddNationWarResult(result);
		NationWarDeclaration declaration = result.declaration;
		declaration.FinishNationWar((result.winNationId == result.offenseNationId) ? true : false, result.regTime);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_Nation);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarDeclaration(declaration.id, declaration.status, declaration.statusUpdateTime));
		dbWork.Schedule();
		AddPastNationWar(declaration);
		_ = result.offenseNationId;
		ServerEvent.SendNationWarFinished(Cache.instance.GetClientPeers(Guid.Empty), result.declaration.id, result.winNationId);
	}

	private void AddNationWarResult(NationWarResult result)
	{
		m_nationWarResults.Add(result.declaration.id, result);
	}

	public NationWarResult GetNationWarResult(int nNationId, DateTime date)
	{
		foreach (NationWarResult result in m_nationWarResults.Values)
		{
			NationWarDeclaration declaration = result.declaration;
			if ((nNationId == declaration.nationId || nNationId == declaration.targetNationId) && !(date != result.regTime.Date))
			{
				return result;
			}
		}
		return null;
	}

	public NationWarResult GetNationWarResult(Guid declarationId)
	{
		if (!m_nationWarResults.TryGetValue(declarationId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveNationWarInstance(Guid declarationId)
	{
		m_nationWarInsts.Remove(declarationId);
	}

	public void Release()
	{
		foreach (NationWarInstance nationWarInst in m_nationWarInsts.Values)
		{
			nationWarInst.Release();
		}
	}

	public List<PDNationWarDeclaration> GetPDNationWarDeclarations()
	{
		List<PDNationWarDeclaration> results = new List<PDNationWarDeclaration>();
		foreach (NationWarDeclaration declaration in m_nationWarDeclarations.Values)
		{
			results.Add(declaration.ToPDNationWarDeclaration());
		}
		return results;
	}

	public List<PDNationWarHistory> GetPDNationWarHistories(DateTime date)
	{
		List<PDNationWarHistory> results = new List<PDNationWarHistory>();
		foreach (NationWarDeclaration pastNationWar in m_pastNationWars.Values)
		{
			if (pastNationWar.regTime.Date > date.AddDays(-7.0))
			{
				results.Add(pastNationWar.ToPDNationWarHistroy());
			}
		}
		return results;
	}
}
