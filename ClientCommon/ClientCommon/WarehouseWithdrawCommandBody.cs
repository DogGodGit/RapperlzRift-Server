namespace ClientCommon;

public class WarehouseWithdrawCommandBody : CommandBody
{
	public int warehouseSlotIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(warehouseSlotIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		warehouseSlotIndex = reader.ReadInt32();
	}
}
