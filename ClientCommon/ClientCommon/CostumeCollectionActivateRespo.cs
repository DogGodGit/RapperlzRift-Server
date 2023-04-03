namespace ClientCommon;

public class CostumeCollectionActivateResponseBody : ResponseBody
{
	public int maxHP;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
