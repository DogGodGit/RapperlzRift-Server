namespace ClientCommon;

public class PDTradeShipAdditionalMonsterInstance : PDMonsterInstance
{
	public int stepNo;

	public override MonsterInstanceType type => MonsterInstanceType.TradeShipAdditionalMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
	}
}
