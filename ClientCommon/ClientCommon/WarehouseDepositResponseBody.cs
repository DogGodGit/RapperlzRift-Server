namespace ClientCommon;

public class WarehouseDepositResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public PDWarehouseSlot[] changedWarehouseSlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(changedWarehouseSlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		changedWarehouseSlots = reader.ReadPDPacketDatas<PDWarehouseSlot>();
	}
}
