namespace ClientCommon;

public class SEBEliteMonsterSpawnEventBody : SEBServerEventBody
{
	public int eliteMonsterId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eliteMonsterId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eliteMonsterId = reader.ReadInt32();
	}
}
