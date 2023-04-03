namespace ClientCommon;

public class WarehouseSlotExtendResponseBody : ResponseBody
{
	public int paidWarehouseSlotCount;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(paidWarehouseSlotCount);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		paidWarehouseSlotCount = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
