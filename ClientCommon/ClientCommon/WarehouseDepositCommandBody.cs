namespace ClientCommon;

public class WarehouseDepositCommandBody : CommandBody
{
	public int[] inventorySlotIndices;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(inventorySlotIndices);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		inventorySlotIndices = reader.ReadInts();
	}
}
