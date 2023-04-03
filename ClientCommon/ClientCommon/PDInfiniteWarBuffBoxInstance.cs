namespace ClientCommon;

public class PDInfiniteWarBuffBoxInstance : PDPacketData
{
	public long instanceId;

	public int id;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(id);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		id = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
