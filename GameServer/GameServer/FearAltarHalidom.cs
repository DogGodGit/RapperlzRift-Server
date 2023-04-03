using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidom : IPickEntry
{
	private FearAltar m_fearAltar;

	private int m_nId;

	private FearAltarHalidomLevel m_level;

	private FearAltarHalidomElemental m_elemental;

	private MonsterArrange m_monsterArrange;

	private int m_nPoint;

	public FearAltar fearAltar => m_fearAltar;

	public int id => m_nId;

	public FearAltarHalidomLevel level => m_level;

	public FearAltarHalidomElemental elemental => m_elemental;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public FearAltarHalidom(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["halidomId"]);
		int nHalidomElementalId = Convert.ToInt32(dr["halidomElementalId"]);
		if (nHalidomElementalId > 0)
		{
			m_elemental = m_fearAltar.GetHalidomElemental(nHalidomElementalId);
			if (m_elemental == null)
			{
				SFLogUtil.Warn(GetType(), "성물원소가 존재하지 않습니다. m_nId = " + m_nId + ", nHalidomElementalId = " + nHalidomElementalId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "성물원소ID가 유효하지 않습니다. m_nId = " + m_nId + ", nHalidomElementalId = " + nHalidomElementalId);
		}
		int nHalidomLevel = Convert.ToInt32(dr["halidomLevel"]);
		if (nHalidomLevel > 0)
		{
			m_level = m_fearAltar.GetHalidomLevel(nHalidomLevel);
			if (m_level == null)
			{
				SFLogUtil.Warn(GetType(), "성물레벨이 존재하지 않습니다. m_nId = " + m_nId + ", nHalidomLevel = " + nHalidomLevel);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "성물레벨이 유효하지 않습니다. m_nId = " + m_nId + ", nHalidomLevel = " + nHalidomLevel);
		}
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
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
	}
}
