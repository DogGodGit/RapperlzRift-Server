namespace ClientCommon;

public class PDMailAttachment : PDPacketData
{
	public int no;

	public int itemId;

	public int itemCount;

	public bool itemOwned;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
		writer.Write(itemId);
		writer.Write(itemCount);
		writer.Write(itemOwned);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt32();
		itemId = reader.ReadInt32();
		itemCount = reader.ReadInt32();
		itemOwned = reader.ReadBoolean();
	}
}
