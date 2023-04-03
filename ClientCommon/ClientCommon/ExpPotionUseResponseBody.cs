using System;

namespace ClientCommon;

public class ExpPotionUseResponseBody : ResponseBody
{
	public DateTime date;

	public int expPotinDailyUseCount;

	public PDInventorySlot changedInventorySlot;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(expPotinDailyUseCount);
		writer.Write(changedInventorySlot);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		expPotinDailyUseCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
