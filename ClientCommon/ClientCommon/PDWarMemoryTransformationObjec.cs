namespace ClientCommon;

public class PDWarMemoryTransformationObjectInstance : PDPacketData
{
	public long instanceId;

	public int objectId;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(objectId);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		objectId = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
