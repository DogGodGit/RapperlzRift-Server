using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountLevelMaster
{
	private int m_nLevel;

	private MountQualityMaster m_qualityMaster;

	private int m_nQualityLevel;

	private int m_nNextLevelUpRequiredSatiety;

	public int level => m_nLevel;

	public MountQualityMaster qualityMaster => m_qualityMaster;

	public int qualityLevel => m_nQualityLevel;

	public int nextLevelUpRequiredSatiety => m_nNextLevelUpRequiredSatiety;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		int nQuality = Convert.ToInt32(dr["quality"]);
		m_qualityMaster = Resource.instance.GetMountQualityMaster(nQuality);
		if (m_qualityMaster == null)
		{
			SFLogUtil.Warn(GetType(), "품질마스터가 존재하지 않습니다. m_nLevel = " + m_nLevel + ", nQuality = " + nQuality);
		}
		m_nQualityLevel = Convert.ToInt32(dr["qualityLevel"]);
		if (m_nQualityLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "품질레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nQualityLevel = " + m_nQualityLevel);
		}
		m_nNextLevelUpRequiredSatiety = Convert.ToInt32(dr["nextLevelUpRequiredSatiety"]);
		if (m_nNextLevelUpRequiredSatiety <= 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업요구포만감이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredSatiety = " + m_nNextLevelUpRequiredSatiety);
		}
	}
}
