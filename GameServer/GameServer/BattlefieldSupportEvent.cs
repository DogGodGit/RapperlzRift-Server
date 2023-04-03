using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BattlefieldSupportEvent
{
	private int m_nStartTime;

	private int m_nEndTime;

	private float m_fSupplySupportQuestExploitPointFactor;

	private float m_fKillExploitPointFactor;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public float supplySupportQuestExploitPointFactor => m_fSupplySupportQuestExploitPointFactor;

	public float killExploitPointFactor => m_fKillExploitPointFactor;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0 || m_nStartTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 유효하지 않습니다. m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0 || m_nEndTime > 86400)
		{
			SFLogUtil.Warn(GetType(), "종료시간이 유효하지 않습니다. m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 종료시간보다 크거나 같습니다. m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
		m_fSupplySupportQuestExploitPointFactor = Convert.ToSingle(dr["supplySupportQuestExploitPointFactor"]);
		if (m_fSupplySupportQuestExploitPointFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "보급지원퀘스트공적점수보상계수가 유효하지 않습니다. m_fSupplySupportQuestExploitPointFactor = " + m_fSupplySupportQuestExploitPointFactor);
		}
		m_fKillExploitPointFactor = Convert.ToSingle(dr["killExploitPointFactor"]);
		if (m_fKillExploitPointFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "킬공적포인트계수가 유효하지 않습니다. m_fKillExploitPointFactor = " + m_fKillExploitPointFactor);
		}
	}

	public bool IsEventTime(float fTime)
	{
		if (fTime >= (float)m_nStartTime)
		{
			return fTime < (float)m_nEndTime;
		}
		return false;
	}
}
