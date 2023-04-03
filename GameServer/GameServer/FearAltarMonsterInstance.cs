using System;
using ClientCommon;

namespace GameServer;

public class FearAltarMonsterInstance : MonsterInstance
{
	private FearAltarStageWaveMonsterArrange m_arrange;

	private FearAltarMonsterAttrFactor m_attrFactor;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.FearAltarMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public FearAltarStageWaveMonsterArrange arrange => m_arrange;

	public void Init(FearAltarInstance fearAltarInst, FearAltarStageWaveMonsterArrange arrange, FearAltarMonsterAttrFactor attrFactor)
	{
		if (fearAltarInst == null)
		{
			throw new ArgumentNullException("fearAltarInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_arrange = arrange;
		m_attrFactor = attrFactor;
		InitMonsterInstance(fearAltarInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDFearAltarMonsterInstance inst = new PDFearAltarMonsterInstance();
		inst.monsterType = m_arrange.monsterType;
		return inst;
	}
}
