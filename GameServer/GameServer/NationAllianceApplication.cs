using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationAllianceApplication
{
	public const int kStatus_Application = 0;

	public const int kStatus_Accept = 1;

	public const int kStatus_Reject = 2;

	private Guid m_id = Guid.Empty;

	private NationInstance m_nationInst;

	private NationInstance m_targetNationInst;

	private long m_lnFund;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Guid id => m_id;

	public NationInstance nationInst => m_nationInst;

	public int nationId => m_nationInst.nationId;

	public NationInstance targetNationInst => m_targetNationInst;

	public int targetNationId => m_targetNationInst.nationId;

	public long fund => m_lnFund;

	public DateTimeOffset regTime => m_regTime;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Cache cache = Cache.instance;
		m_id = SFDBUtil.ToGuid(dr["applicationId"], Guid.Empty);
		int nNationId = Convert.ToInt32(dr["nationId"]);
		if (nNationId > 0)
		{
			m_nationInst = cache.GetNationInstance(nNationId);
			if (m_nationInst == null)
			{
				throw new Exception(string.Concat("국가인스턴스가 존재하지 않습니다. m_id = ", m_id, ", nNationId = ", nNationId));
			}
			int nTargetNationId = Convert.ToInt32(dr["targetNationId"]);
			if (nTargetNationId > 0)
			{
				m_targetNationInst = cache.GetNationInstance(nTargetNationId);
				if (m_targetNationInst == null)
				{
					throw new Exception(string.Concat("대상국가인스턴스가 존재하지 않습니다. m_id = ", m_id, ", nTargetNationId = ", nTargetNationId));
				}
				m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"]);
				return;
			}
			throw new Exception(string.Concat("대상국가ID가 유효하지 않습니다. m_id = ", m_id, ", nTargetNationId = ", nTargetNationId));
		}
		throw new Exception(string.Concat("국가ID가 유효하지 않습니다. m_id = ", m_id, ", nNationId = ", nNationId));
	}

	public void Init(NationInstance nationInst, NationInstance targetNationInst, long lnFund, DateTimeOffset regTime)
	{
		if (nationInst == null)
		{
			throw new ArgumentNullException("nationInst");
		}
		if (targetNationInst == null)
		{
			throw new ArgumentNullException("targetNationInst");
		}
		m_id = Guid.NewGuid();
		m_nationInst = nationInst;
		m_targetNationInst = targetNationInst;
		m_lnFund = lnFund;
		m_regTime = regTime;
	}

	public PDNationAllianceApplication ToPDNationAllianceApplication()
	{
		PDNationAllianceApplication inst = new PDNationAllianceApplication();
		inst.id = (Guid)m_id;
		inst.nationId = nationId;
		inst.targetNationId = targetNationId;
		return inst;
	}
}
