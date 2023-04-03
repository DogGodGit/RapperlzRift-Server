using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountAwakeningLevel
{
	private Mount m_mount;

	private MountAwakeningLevelMaster m_levelMaster;

	private int m_nNextLevelUpAwakeningExp;

	public Mount mount => m_mount;

	public MountAwakeningLevelMaster levelMaster => m_levelMaster;

	public int nextLevelUpAwakeningExp => m_nNextLevelUpAwakeningExp;

	public MountAwakeningLevel(Mount mount)
	{
		if (mount == null)
		{
			throw new ArgumentNullException("mount");
		}
		m_mount = mount;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nLevel = Convert.ToInt32(dr["awakeningLevel"]);
		m_levelMaster = Resource.instance.GetMountAwakeningLevelMaster(nLevel);
		if (m_levelMaster == null)
		{
			SFLogUtil.Warn(GetType(), "레벨마스터가 존재하지 않습니다. nLevel = " + nLevel);
		}
		m_nNextLevelUpAwakeningExp = Convert.ToInt32(dr["nextLevelUpRequiredAwakeningExp"]);
		if (m_nNextLevelUpAwakeningExp < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요각성경험치가 유효하지 않습니다. nLevel = " + nLevel + ", m_nNextLevelUpAwakeningExp = " + m_nNextLevelUpAwakeningExp);
		}
	}
}
