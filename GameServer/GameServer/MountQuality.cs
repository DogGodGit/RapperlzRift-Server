using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountQuality
{
	private Mount m_mount;

	private MountQualityMaster m_qualityMaster;

	private int m_nPotionAttrMaxCount;

	public Mount mount
	{
		get
		{
			return m_mount;
		}
		set
		{
			m_mount = value;
		}
	}

	public MountQualityMaster qualityMaster => m_qualityMaster;

	public int potionAttrMaxCount => m_nPotionAttrMaxCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nQuality = Convert.ToInt32(dr["quality"]);
		m_qualityMaster = Resource.instance.GetMountQualityMaster(nQuality);
		if (m_qualityMaster == null)
		{
			SFLogUtil.Warn(GetType(), "품질마스터가 존재하지 않습니다. nQuality = " + nQuality);
		}
		m_nPotionAttrMaxCount = Convert.ToInt32(dr["potionAttrMaxCount"]);
		if (m_nPotionAttrMaxCount < 0)
		{
			SFLogUtil.Warn(GetType(), "물약속성최대카운트가 유효하지 않습니다. nQuality = " + nQuality + ", m_nPotionAttrMaxCount = " + m_nPotionAttrMaxCount);
		}
	}
}
