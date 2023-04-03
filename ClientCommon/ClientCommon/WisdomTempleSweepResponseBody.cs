using System;

namespace ClientCommon;

public class WisdomTempleSweepResponseBody : ResponseBody
{
	public DateTime date;

	public int stamina;

	public int playCount;

	public int freeSweepDailyCount;

	public PDInventorySlot[] changedInventorySlots;

	public PDItemBooty booty;

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
		writer.Write(changedInventorySlots);
		writer.Write(booty);
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
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		booty = reader.ReadPDBooty<PDItemBooty>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
