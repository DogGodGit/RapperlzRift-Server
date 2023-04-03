using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelicMonsterAttrFactor
{
	private AncientRelic m_ancientRelic;

	private int m_nAverageHeroLevel;

	private float m_fTrapDamageFactor;

	private float m_fNormalMonsterMaxHpFactor;

	private float m_fNormalMonsterOffenseFactor;

	private float m_fBossMonsterMaxHpFactor;

	private float m_fBossMonsterOffenseFactor;

	public AncientRelic ancientRelic => m_ancientRelic;

	public int averageHeroLevel => m_nAverageHeroLevel;

	public float trapDamageFactor => m_fTrapDamageFactor;

	public float normalMonsterMaxHpFactor => m_fNormalMonsterMaxHpFactor;

	public float normalMonsterOffenseFactor => m_fNormalMonsterOffenseFactor;

	public float bossMonsterMaxHpFactor => m_fBossMonsterMaxHpFactor;

	public float bossMonsterOffenseFactor => m_fBossMonsterOffenseFactor;

	public AncientRelicMonsterAttrFactor(AncientRelic ancientRelic)
	{
		m_ancientRelic = ancientRelic;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAverageHeroLevel = Convert.ToInt32(dr["averageHeroLevel"]);
		if (m_nAverageHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅평균레벨이 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel);
		}
		m_fTrapDamageFactor = Convert.ToSingle(dr["trapDamageFactor"]);
		if (m_fTrapDamageFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "함정데이지계수가 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel + ", m_fTrapDamageFactor = " + m_fTrapDamageFactor);
		}
		m_fNormalMonsterMaxHpFactor = Convert.ToSingle(dr["normalMonsterMaxHpFactor"]);
		if (m_fNormalMonsterMaxHpFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "일반몬스터최대HP계수가 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel + ", m_fNormalMonsterMaxHpFactor = " + m_fNormalMonsterMaxHpFactor);
		}
		m_fNormalMonsterOffenseFactor = Convert.ToSingle(dr["normalMonsterOffenseFactor"]);
		if (m_fNormalMonsterOffenseFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "일반몬스터공격력계수가 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel + ", m_fNormalMonsterOffenseFactor = " + m_fNormalMonsterOffenseFactor);
		}
		m_fBossMonsterMaxHpFactor = Convert.ToSingle(dr["bossMonsterMaxHpFactor"]);
		if (m_fBossMonsterMaxHpFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터최대HP계수가 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel + ", m_fBossMonsterMaxHpFactor = " + m_fBossMonsterMaxHpFactor);
		}
		m_fBossMonsterOffenseFactor = Convert.ToSingle(dr["bossMonsterOffenseFactor"]);
		if (m_fBossMonsterOffenseFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터공격력계수가 유효하지 않습니다. m_nAverageHeroLevel = " + m_nAverageHeroLevel + ", m_fBossMonsterOffenseFactor = " + m_fBossMonsterOffenseFactor);
		}
	}
}
