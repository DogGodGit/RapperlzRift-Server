namespace ClientCommon;

public class RuneSocketUnmountCommandBody : CommandBody
{
	public int subGearId;

	public int socketIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
		writer.Write(socketIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
		socketIndex = reader.ReadInt32();
	}
}
