namespace ClientCommon;

public class PDWisdomTempleColorMatchingObjectInstance : PDPacketData
{
	public long instanceId;

	public int objectId;

	public int row;

	public int col;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(objectId);
		writer.Write(row);
		writer.Write(col);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		objectId = reader.ReadInt32();
		row = reader.ReadInt32();
		col = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
