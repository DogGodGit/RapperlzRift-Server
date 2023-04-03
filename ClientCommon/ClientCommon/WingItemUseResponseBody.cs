namespace ClientCommon;

public class WingItemUseResponseBody : ResponseBody
{
	public int maxHP;

	public PDHeroWing addedWing;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(addedWing);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		addedWing = reader.ReadPDPacketData<PDHeroWing>();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
