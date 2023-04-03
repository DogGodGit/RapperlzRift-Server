namespace ClientCommon;

public class SEBRuinsReclaimMonsterSummonEventBody : SEBServerEventBody
{
	public PDRuinsReclaimSummonMonsterInstance[] monsterInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInst = reader.ReadPDMonsterInstances<PDRuinsReclaimSummonMonsterInstance>();
	}
}
