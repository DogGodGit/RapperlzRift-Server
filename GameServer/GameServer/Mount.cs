using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class Mount
{
	private int m_nId;

	private string m_sNameKey;

	private float m_fMoveSpeed;

	private List<MountLevel> m_levels = new List<MountLevel>();

	private List<MountQuality> m_qualities = new List<MountQuality>();

	private List<MountAwakeningLevel> m_awakeningLevels = new List<MountAwakeningLevel>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public float moveSpeed => m_fMoveSpeed;

	public MountLevel lastMountLevel => m_levels.LastOrDefault();

	public MountAwakeningLevel lastawakeningLevel => m_awakeningLevels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["mountId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "탈것ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_fMoveSpeed = Convert.ToSingle(dr["moveSpeed"]);
		if (m_fMoveSpeed < 0f)
		{
			SFLogUtil.Warn(GetType(), "탈것 이동속도가 유효하지 않습니다. m_nId = " + m_nId + ", m_fMoveSpeed = " + m_fMoveSpeed);
		}
	}

	public void AddLevel(MountLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
		level.mount = this;
	}

	public MountLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public void AddQuality(MountQuality quality)
	{
		if (quality == null)
		{
			throw new ArgumentNullException("quality");
		}
		m_qualities.Add(quality);
		quality.mount = this;
	}

	public MountQuality GetQuality(int nQuality)
	{
		int nIndex = nQuality - 1;
		if (nIndex < 0 || nIndex >= m_qualities.Count)
		{
			return null;
		}
		return m_qualities[nIndex];
	}

	public void AddAwakeningLevel(MountAwakeningLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_awakeningLevels.Add(level);
	}

	public MountAwakeningLevel GetAwakeningLevel(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_awakeningLevels.Count)
		{
			return null;
		}
		return m_awakeningLevels[nLevel];
	}
}
