using System;

namespace ClientCommon;

public class DimensionRaidQuestCompleteResponseBody : ResponseBody
{
	public long acquiredExp;

	public int acquiredExploitPoint;

	public int level;

	public long exp;

	public int exploitPoint;

	public int maxHp;

	public int hp;

	public DateTime date;

	public int dailyExploitPoint;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(acquiredExploitPoint);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(exploitPoint);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(date);
		writer.Write(dailyExploitPoint);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		acquiredExploitPoint = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		exploitPoint = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		date = reader.ReadDateTime();
		dailyExploitPoint = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
