using System;
using ClientCommon;

namespace GameServer;

public class ProofOfValorNormalMonsterInstance : MonsterInstance
{
	private ProofOfValorNormalMonsterArrange m_arrange;

	private ProofOfValorMonsterAttrFactor m_attrFactor;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ProofOfValorNormalMonster;

	public ProofOfValorNormalMonsterArrange arrange => m_arrange;

	public void Init(ProofOfValorInstance proofOfValorInst, ProofOfValorNormalMonsterArrange arrange)
	{
		if (proofOfValorInst == null)
		{
			throw new ArgumentNullException("proofOfValorInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_attrFactor = proofOfValorInst.proofOfValor.GetMonsterAttrFactor(proofOfValorInst.heroProofOfValorInst.level);
		InitMonsterInstance(proofOfValorInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDProofOfValorNormalMonsterInstance();
	}
}
