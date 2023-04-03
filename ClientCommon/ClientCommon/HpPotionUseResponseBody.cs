namespace ClientCommon;

public class HpPotionUseResponseBody : ResponseBody
{
	public PDInventorySlot changedInventorySlot;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		hp = reader.ReadInt32();
	}
}
