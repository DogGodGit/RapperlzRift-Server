using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorBuffBox
{
	private ProofOfValor m_proofOfValor;

	private int m_nId;

	private float m_fOffenseFactor;

	private float m_fPhysicalDefenseFactor;

	private float m_fHpRecoveryFactor;

	private Dictionary<int, ProofOfValorBuffBoxArrange> m_arranges = new Dictionary<int, ProofOfValorBuffBoxArrange>();

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int id => m_nId;

	public float offenseFactor => m_fOffenseFactor;

	public float physicalDefenseFactor => m_fPhysicalDefenseFactor;

	public float hpRecoveryFactor => m_fHpRecoveryFactor;

	public ProofOfValorBuffBox(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["buffBoxId"]);
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
		if (m_fOffenseFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "공격력계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fOffenseFactor = " + m_fOffenseFactor);
		}
		m_fPhysicalDefenseFactor = Convert.ToSingle(dr["physicalDefenseFactor"]);
		if (m_fPhysicalDefenseFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "물리방어력계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fPhysicalDefenseFactor = " + m_fPhysicalDefenseFactor);
		}
		m_fHpRecoveryFactor = Convert.ToSingle(dr["hpRecoveryFactor"]);
		if (m_fHpRecoveryFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "HP회복계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fHpRecoveryFactor = " + m_fHpRecoveryFactor);
		}
	}

	public void AddArrange(ProofOfValorBuffBoxArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arranges.Add(arrange.id, arrange);
	}

	public ProofOfValorBuffBoxArrange GetArrange(int nArrangeId)
	{
		if (!m_arranges.TryGetValue(nArrangeId, out var value))
		{
			return null;
		}
		return value;
	}
}
