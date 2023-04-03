using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class Artifact
{
	private int m_nNo;

	private HashSet<int> m_attrs = new HashSet<int>();

	private List<ArtifactLevel> m_levels = new List<ArtifactLevel>();

	public int no => m_nNo;

	public ArtifactLevel lastLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["artifactNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "아티팩트번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
	}

	public void AddAttr(int nAttrId)
	{
		m_attrs.Add(nAttrId);
	}

	public bool IsAttr(int nAttrId)
	{
		return m_attrs.Contains(nAttrId);
	}

	public void AddLevel(ArtifactLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
	}

	public ArtifactLevel GetLevel(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nLevel];
	}
}
