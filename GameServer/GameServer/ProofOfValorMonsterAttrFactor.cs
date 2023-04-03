using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorMonsterAttrFactor
{
	private ProofOfValor m_proofOfValor;

	private int m_nHeroLevel;

	private float m_fMaxHpFactor;

	private float m_fOffenseFactor;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int heroLevel => m_nHeroLevel;

	public float maxHpFactor => m_fMaxHpFactor;

	public float offenseFactor => m_fOffenseFactor;

	public ProofOfValorMonsterAttrFactor(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHeroLevel = Convert.ToInt32(dr["heroLevel"]);
		if (m_nHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅레벨이 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel);
		}
		m_fMaxHpFactor = Convert.ToSingle(dr["maxHpFactor"]);
		if (m_fMaxHpFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "최대HP계수가 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", m_fMaxHpFactor = " + m_fMaxHpFactor);
		}
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
		if (m_fOffenseFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "공격력계수가 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", m_fOffenseFactor = " + m_fOffenseFactor);
		}
	}
}
