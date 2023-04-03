using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class EliteMonster : IPickEntry
{
	private int m_nId;

	private EliteMonsterMaster m_master;

	private int m_nPoint;

	private int m_nStarGrade;

	private int m_nAttrId;

	private MonsterArrange m_monsterArrange;

	private List<EliteMonsterKillAttrValue> m_killAttrValues = new List<EliteMonsterKillAttrValue>();

	public int id => m_nId;

	public EliteMonsterMaster master => m_master;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public int starGrade => m_nStarGrade;

	public int attrId => m_nAttrId;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["eliteMonsterId"]);
		int nEliteMonsterMasterId = Convert.ToInt32(dr["eliteMonsterMasterId"]);
		if (nEliteMonsterMasterId > 0)
		{
			m_master = Resource.instance.GetEliteMonsterMaster(nEliteMonsterMasterId);
			if (m_master == null)
			{
				SFLogUtil.Warn(GetType(), "정예몬스터마스터가 존재하지 않습니다. m_nId = " + m_nId + ", nEliteMonsterMasterId = " + nEliteMonsterMasterId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "정예몬스터마스터ID가 유효하지 않습니다. m_nId = " + m_nId + ", nEliteMonsterMasterId = " + nEliteMonsterMasterId);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_nStarGrade = Convert.ToInt32(dr["starGrade"]);
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
	}

	public void AddKillAttrValue(EliteMonsterKillAttrValue killAttrValue)
	{
		if (killAttrValue == null)
		{
			throw new ArgumentNullException("killAttrValue");
		}
		m_killAttrValues.Add(killAttrValue);
	}

	public EliteMonsterKillAttrValue GetKillAttrValue_LessThanOrEqualValue(int nKillCount)
	{
		EliteMonsterKillAttrValue value = null;
		foreach (EliteMonsterKillAttrValue killAttrValue in m_killAttrValues)
		{
			int nKillAttrValueKillCount = killAttrValue.killCount;
			if (nKillCount >= nKillAttrValueKillCount)
			{
				value = killAttrValue;
				continue;
			}
			return value;
		}
		return value;
	}

	public EliteMonsterKillAttrValue GetKillAttrValue(int nKillCount)
	{
		foreach (EliteMonsterKillAttrValue killAttrValue in m_killAttrValues)
		{
			if (killAttrValue.killCount == nKillCount)
			{
				return killAttrValue;
			}
		}
		return null;
	}

	public bool ContainsKillAttrValue(int nKillCount)
	{
		return GetKillAttrValue(nKillCount) != null;
	}
}
