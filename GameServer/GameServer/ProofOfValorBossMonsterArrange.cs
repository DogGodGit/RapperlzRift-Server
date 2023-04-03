using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorBossMonsterArrange : IPickEntry
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private ProofOfValor m_proofOfValor;

	private int m_nId;

	private int m_nPoint;

	private bool m_bIsSpecial;

	private MonsterArrange m_monsterArrange;

	private ProofOfValorCreatureCardPool m_creatureCardPool;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nRewardSoulPowder;

	private int m_nSpecialRewardSoulPowder;

	private List<ProofOfValorNormalMonsterArrange> m_normalMonsterArranges = new List<ProofOfValorNormalMonsterArrange>();

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int id => m_nId;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public bool isSpecial => m_bIsSpecial;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public ProofOfValorCreatureCardPool creatureCardPool => m_creatureCardPool;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int rewardSoulPowder => m_nRewardSoulPowder;

	public int specialRewardSoulPowder => m_nSpecialRewardSoulPowder;

	public List<ProofOfValorNormalMonsterArrange> normalMonsterArranges => m_normalMonsterArranges;

	public ProofOfValorBossMonsterArrange(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["proofOfValorBossMonsterArrangeId"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_bIsSpecial = Convert.ToBoolean(dr["isSpecial"]);
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
		int nCreatureCardPoolId = Convert.ToInt32(dr["creatureCardPoolId"]);
		if (nCreatureCardPoolId > 0)
		{
			m_creatureCardPool = m_proofOfValor.GetCreatureCardPool(nCreatureCardPoolId);
			if (m_creatureCardPool == null)
			{
				SFLogUtil.Warn(GetType(), "크리처카드풀이 존재하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId + ", nCreatureCardPoolId = " + nCreatureCardPoolId);
			}
		}
		else if (nCreatureCardPoolId < 0)
		{
			SFLogUtil.Warn(GetType(), "크리처카드풀ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId + ", nCreatureCardPoolId = " + nCreatureCardPoolId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nRewardSoulPowder = Convert.ToInt32(dr["rewardSoulPowder"]);
		if (m_nRewardSoulPowder < 0)
		{
			SFLogUtil.Warn(GetType(), "보상영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRewardSoulPowder = " + m_nRewardSoulPowder);
		}
		m_nSpecialRewardSoulPowder = Convert.ToInt32(dr["specialRewardSoulPowder"]);
		if (m_nSpecialRewardSoulPowder < 0)
		{
			SFLogUtil.Warn(GetType(), "스페셜보상영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nSpecialRewardSoulPowder = " + m_nSpecialRewardSoulPowder);
		}
	}

	public void AddNormalMonsterArrange(ProofOfValorNormalMonsterArrange normalMonsterArrange)
	{
		if (normalMonsterArrange == null)
		{
			throw new ArgumentNullException("normalMonsterArrange");
		}
		m_normalMonsterArranges.Add(normalMonsterArrange);
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
