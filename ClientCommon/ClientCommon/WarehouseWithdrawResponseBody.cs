namespace ClientCommon;

public class WarehouseWithdrawResponseBody : ResponseBody
{
	public PDInventorySlot[] cangedInvetorySlots;

	public PDWarehouseSlot changedWarehouseSlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cangedInvetorySlots);
		writer.Write(changedWarehouseSlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cangedInvetorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		changedWarehouseSlot = reader.ReadPDPacketData<PDWarehouseSlot>();
	}
}
