using System;

namespace ClientCommon;

public class MainGearEnchantResponseBody : ResponseBody
{
	public DateTime date;

	public bool isSuccess;

	public int enchantLevel;

	public int maxEquippedMainGearEnchantLevel;

	public int enchantDailyCount;

	public PDInventorySlot[] changedInventorySlots;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(isSuccess);
		writer.Write(enchantLevel);
		writer.Write(maxEquippedMainGearEnchantLevel);
		writer.Write(enchantDailyCount);
		writer.Write(changedInventorySlots);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		isSuccess = reader.ReadBoolean();
		enchantLevel = reader.ReadInt32();
		maxEquippedMainGearEnchantLevel = reader.ReadInt32();
		enchantDailyCount = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
