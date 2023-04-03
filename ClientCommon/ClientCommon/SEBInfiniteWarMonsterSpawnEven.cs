namespace ClientCommon;

public class SEBInfiniteWarMonsterSpawnEventBody : SEBServerEventBody
{
	public PDInfiniteWarMonsterInstance[] monsterInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInsts = reader.ReadPDMonsterInstances<PDInfiniteWarMonsterInstance>();
	}
}
