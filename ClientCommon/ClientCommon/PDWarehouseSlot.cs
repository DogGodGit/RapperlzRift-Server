namespace ClientCommon;

public class PDWarehouseSlot : PDPacketData
{
	public int index;

	public PDWarehouseObject warehouseObject;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(warehouseObject);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		warehouseObject = reader.ReadPDWarehouseObject();
	}
}
