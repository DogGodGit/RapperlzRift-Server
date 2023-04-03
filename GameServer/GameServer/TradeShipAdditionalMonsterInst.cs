using System;
using ClientCommon;

namespace GameServer;

public class TradeShipAdditionalMonsterInstance : MonsterInstance
{
	private TradeShipAdditionalMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.TradeShipAdditionalMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public TradeShipAdditionalMonsterArrange arrange => m_arrange;

	public void Init(TradeShipInstance tradeShipInst, TradeShipAdditionalMonsterArrange arrange)
	{
		if (tradeShipInst == null)
		{
			throw new ArgumentNullException("tradeShipInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(tradeShipInst, arrange.position, arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDTradeShipAdditionalMonsterInstance inst = new PDTradeShipAdditionalMonsterInstance();
		inst.stepNo = m_arrange.entry.step.no;
		return inst;
	}
}
