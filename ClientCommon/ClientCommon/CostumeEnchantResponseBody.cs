namespace ClientCommon;

public class CostumeEnchantResponseBody : ResponseBody
{
	public int enchantLevel;

	public int luckyValue;

	public int maxHP;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(enchantLevel);
		writer.Write(luckyValue);
		writer.Write(maxHP);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		enchantLevel = reader.ReadInt32();
		luckyValue = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
