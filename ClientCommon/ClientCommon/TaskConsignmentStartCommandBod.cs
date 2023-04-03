namespace ClientCommon;

public class TaskConsignmentStartCommandBody : CommandBody
{
	public int consignmentId;

	public int useExpItemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(consignmentId);
		writer.Write(useExpItemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		consignmentId = reader.ReadInt32();
		useExpItemId = reader.ReadInt32();
	}
}
