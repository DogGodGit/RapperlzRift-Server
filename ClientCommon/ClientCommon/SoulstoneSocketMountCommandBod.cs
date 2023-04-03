namespace ClientCommon;

public class SoulstoneSocketMountCommandBody : CommandBody
{
	public int subGearId;

	public int socketIndex;

	public int itemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
		writer.Write(socketIndex);
		writer.Write(itemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
		socketIndex = reader.ReadInt32();
		itemId = reader.ReadInt32();
	}
}
