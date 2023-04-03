namespace ClientCommon;

public class SEBMonsterSpawnEventBody : SEBServerEventBody
{
	public PDMonsterInstance monster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monster);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monster = reader.ReadPDMonsterInstance();
	}
}
