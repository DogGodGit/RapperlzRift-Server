using System;
using ClientCommon;

namespace GameServer;

public class TradeShipMonsterInstance : MonsterInstance
{
	private TradeShipMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.TradeShipMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public TradeShipMonsterArrange arrange => m_arrange;

	public void Init(TradeShipInstance tradeShipInst, TradeShipMonsterArrange arrange)
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
		PDTradeShipMonsterInstance inst = new PDTradeShipMonsterInstance();
		inst.stepNo = m_arrange.step.no;
		return inst;
	}
}
