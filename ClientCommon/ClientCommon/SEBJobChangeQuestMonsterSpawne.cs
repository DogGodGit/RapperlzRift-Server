namespace ClientCommon;

public class SEBJobChangeQuestMonsterSpawnedEventBody : SEBServerEventBody
{
	public long instanceId;

	public PDVector3 position;

	public float remainingLifetime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(position);
		writer.Write(remainingLifetime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		position = reader.ReadPDVector3();
		remainingLifetime = reader.ReadSingle();
	}
}
