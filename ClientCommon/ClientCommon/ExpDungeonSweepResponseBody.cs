using System;

namespace ClientCommon;

public class ExpDungeonSweepResponseBody : ResponseBody
{
	public DateTime date;

	public int stamina;

	public int playCount;

	public int freeSweepDailyCount;

	public PDInventorySlot changedInventorySlot;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(stamina);
		writer.Write(playCount);
		writer.Write(freeSweepDailyCount);
		writer.Write(changedInventorySlot);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		stamina = reader.ReadInt32();
		playCount = reader.ReadInt32();
		freeSweepDailyCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
