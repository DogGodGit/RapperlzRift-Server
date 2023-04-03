namespace ClientCommon;

public class OwnDiaItemUseResponseBody : ResponseBody
{
	public int ownDia;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ownDia);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ownDia = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
