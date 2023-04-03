namespace ClientCommon;

public class RuneSocketMountResponseBody : ResponseBody
{
	public PDInventorySlot changedInventorySlot;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		maxHp = reader.ReadInt32();
	}
}
