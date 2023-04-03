namespace ClientCommon;

public class HeroAttrPotionUseAllResponseBody : ResponseBody
{
	public int maxHP;

	public PDHeroPotionAttr[] changedPotionAttrs;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(changedPotionAttrs);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		changedPotionAttrs = reader.ReadPDPacketDatas<PDHeroPotionAttr>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
