namespace ClientCommon;

public class InfiniteWarBuffBoxAcquireCommandBody : CommandBody
{
	public long instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
	}
}
