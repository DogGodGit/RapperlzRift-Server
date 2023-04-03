using System;

namespace ClientCommon;

public class RetrieveGoldResponseBody : ResponseBody
{
	public DateTime date;

	public int retrievalCount;

	public long gold;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(retrievalCount);
		writer.Write(gold);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		retrievalCount = reader.ReadInt32();
		gold = reader.ReadInt64();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
