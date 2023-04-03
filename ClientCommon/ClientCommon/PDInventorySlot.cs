namespace ClientCommon;

public class PDInventorySlot : PDPacketData
{
	public int index;

	public PDInventoryObject inventoryObject;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(inventoryObject);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		inventoryObject = reader.ReadPDInventoryObject();
	}
}
