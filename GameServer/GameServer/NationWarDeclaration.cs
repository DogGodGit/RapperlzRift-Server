using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarDeclaration
{
	public const int kStatus_Declaration = 0;

	public const int kStatus_Win = 1;

	public const int kStatus_Lose = 2;

	public const int kLogType_System = 1;

	public const int kLogType_User = 2;

	public const int kNationWarStatus_Declaration = 0;

	public const int kNationWarStatus_War = 1;

	public const int kNationWarStatus_Finished = 2;

	private Guid m_id = Guid.Empty;

	private Nation m_nation;

	private Nation m_targetNation;

	private int m_nStatus;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_statusUpdateTime = DateTimeOffset.MinValue;

	private NationWarInstance m_nationWarInst;

	public Guid id => m_id;

	public Nation nation => m_nation;

	public int nationId => m_nation.id;

	public Nation targetNation => m_targetNation;

	public int targetNationId => m_targetNation.id;

	public int status => m_nStatus;

	public DateTimeOffset regTime => m_regTime;

	public DateTimeOffset statusUpdateTime => m_statusUpdateTime;

	public NationWarInstance nationWarInst => m_nationWarInst;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_id = SFDBUtil.ToGuid(dr["declarationId"], Guid.Empty);
		int nNationId = Convert.ToInt32(dr["nationId"]);
		if (nNationId > 0)
		{
			m_nation = res.GetNation(nNationId);
			if (m_nation == null)
			{
				SFLogUtil.Warn(GetType(), "국가가 존재하지 않습니다. nNationId = " + nNationId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "국가ID가 유효하지 않습니다. nNationId = " + nNationId);
		}
		int nTargetNationId = Convert.ToInt32(dr["targetNationId"]);
		if (nTargetNationId > 0)
		{
			m_targetNation = res.GetNation(nTargetNationId);
			if (m_targetNation == null)
			{
				SFLogUtil.Warn(GetType(), "목표국가가 존재하지 않습니다. nTargetNationId = " + nTargetNationId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "목표국가ID가 유효하지 않습니다. nTargetNationId = " + nTargetNationId);
		}
		m_nStatus = Convert.ToInt32(dr["status"]);
		m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
		m_statusUpdateTime = SFDBUtil.ToDateTimeOffset(dr["statusUpdateTime"], DateTimeOffset.MinValue);
	}

	public void Init(Nation nation, Nation targetNation, DateTimeOffset regTime)
	{
		if (nation == null)
		{
			throw new ArgumentNullException("nation");
		}
		if (targetNation == null)
		{
			throw new ArgumentNullException("targetNation");
		}
		m_id = Guid.NewGuid();
		m_nation = nation;
		m_targetNation = targetNation;
		m_nStatus = 0;
		m_regTime = regTime;
	}

	public void StartNationWar(NationWarInstance nationWarInst)
	{
		if (nationWarInst == null)
		{
			throw new ArgumentNullException("nationWarInst");
		}
		m_nationWarInst = nationWarInst;
	}

	public void FinishNationWar(bool bIsWin, DateTimeOffset time)
	{
		if (bIsWin)
		{
			m_nStatus = 1;
		}
		else
		{
			m_nStatus = 2;
		}
		m_nationWarInst = null;
		m_statusUpdateTime = time;
	}

	public PDNationWarDeclaration ToPDNationWarDeclaration()
	{
		PDNationWarDeclaration inst = new PDNationWarDeclaration();
		inst.declarationId = (Guid)m_id;
		inst.nationId = nationId;
		inst.targetNationId = targetNationId;
		inst.time = (DateTimeOffset)m_regTime;
		if (m_nationWarInst != null)
		{
			inst.status = 1;
		}
		else
		{
			inst.status = ((m_nStatus != 0) ? 2 : 0);
		}
		return inst;
	}

	public PDNationWarHistory ToPDNationWarHistroy()
	{
		PDNationWarHistory inst = new PDNationWarHistory();
		inst.date = (DateTime)m_statusUpdateTime.Date;
		inst.offenseNationId = nationId;
		inst.defenseNationId = targetNationId;
		inst.winNationId = ((m_nStatus == 1) ? nationId : targetNationId);
		return inst;
	}
}
