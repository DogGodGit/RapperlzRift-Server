using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationAllianceBreakCommandHandler : InGameCommandHandler<NationAllianceBreakCommandBody, NationAllianceBreakResponseBody>
{
	public const short kResult_NotExistTargetAlliance = 101;

	public const short kResult_NotAlliance = 102;

	public const short kResult_NotSameAlliance = 103;

	public const short kResult_NotNationAllianceRenounceUnavailableTimeExpired = 104;

	public const short kResult_NationAllianceUnavailableTime = 105;

	public const short kResult_NotAuthority = 106;

	private NationAlliance m_nationAlliance;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		Cache cache = Cache.instance;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid nationAllianceId = (Guid)m_body.nationAllianceId;
		if (cache.GetNationAlliance(nationAllianceId) == null)
		{
			throw new CommandHandleException(101, "해당 국가동맹이 존재하지 않습니다.");
		}
		m_nationAlliance = m_myHero.nationInst.alliance;
		if (m_nationAlliance == null)
		{
			throw new CommandHandleException(102, "현재 동맹을 맺은 국가가 없습니다.");
		}
		if (nationAllianceId != m_nationAlliance.id)
		{
			throw new CommandHandleException(103, "해당 동맹ID가 영웅국가의 동맹ID가 아닙니다.");
		}
		if (m_nationAlliance.GetAllianceRenounceAvailableRemainingTime(m_currentTime) > 0f)
		{
			throw new CommandHandleException(104, "국가동맹파기불가기간이 만료되지 않았습니다.");
		}
		if (Resource.instance.IsNationAllianceUnavailableTime(m_currentTime))
		{
			throw new CommandHandleException(105, "현재 국가동맹관련 명령을 수행할 수 없는 시간입니다.");
		}
		NationNoblesse nationNoblesse = m_myHero.nationNoblesse;
		nationNoblesse = m_myHero.nationNoblesse;
		if (nationNoblesse == null)
		{
			throw new CommandHandleException(106, "영웅이 관직에 임명되지 않았습니다.");
		}
		if (!nationNoblesse.nationAllianceEnabled)
		{
			throw new CommandHandleException(106, "영웅이 국가동맹신청 권한이 없습니다.");
		}
		foreach (NationInstance nationInst in m_nationAlliance.nationInsts.Values)
		{
			nationInst.ClearAlliance();
		}
		cache.BreakNationAlliance(m_nationAlliance, m_myHero.id);
		SaveToDB();
		SaveToDB_Log();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		int nNationId = m_myHero.nationId;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nNationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(m_nationAlliance.GetAllianceNationId(nNationId)));
		foreach (int nAllianceNationId in m_nationAlliance.nationInsts.Keys)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Alliance(nAllianceNationId, Guid.Empty));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteAlliance(m_nationAlliance.id));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nNationId = m_myHero.nationId;
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationAllianceBrokenLog(m_nationAlliance.id, nNationId, m_nationAlliance.GetAllianceNationId(nNationId), m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
