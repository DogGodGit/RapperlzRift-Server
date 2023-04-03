namespace ClientCommon;

public class SEBTradeShipStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public PDTradeShipMonsterInstance[] monsterInsts;

	public PDTradeShipAdditionalMonsterInstance[] additionalMonsterInsts;

	public PDTradeShipObjectInstance[] objectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(monsterInsts);
		writer.Write(additionalMonsterInsts);
		writer.Write(objectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		monsterInsts = reader.ReadPDMonsterInstances<PDTradeShipMonsterInstance>();
		additionalMonsterInsts = reader.ReadPDMonsterInstances<PDTradeShipAdditionalMonsterInstance>();
		objectInsts = reader.ReadPDMonsterInstances<PDTradeShipObjectInstance>();
	}
}
