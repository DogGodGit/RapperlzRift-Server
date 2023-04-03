using System;
using ClientCommon;

namespace GameServer;

public class TradeShipObjectInstance : MonsterInstance
{
	private TradeShipObject m_obj;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.TradeShipObject;

	public override Monster monster => m_obj.monsterArrange.monster;

	public TradeShipObject obj => m_obj;

	public void Init(TradeShipInstance tradeShipInst, TradeShipObject obj)
	{
		if (tradeShipInst == null)
		{
			throw new ArgumentNullException("tradeShipInst");
		}
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		m_obj = obj;
		InitMonsterInstance(tradeShipInst, obj.position, obj.yRotation);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDTradeShipObjectInstance();
	}
}
