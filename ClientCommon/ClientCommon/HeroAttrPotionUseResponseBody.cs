namespace ClientCommon;

public class HeroAttrPotionUseResponseBody : ResponseBody
{
	public int maxHP;

	public int potionAttrCount;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(potionAttrCount);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		potionAttrCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
