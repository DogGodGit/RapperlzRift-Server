using System;

namespace ClientCommon;

public class GoldDungeonSweepResponseBody : ResponseBody
{
	public DateTime date;

	public int stamina;

	public int playCount;

	public int freeSweepDailyCount;

	public PDInventorySlot changedInventorySlot;

	public long rewardGold;

	public long gold;

	public long maxGold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(stamina);
		writer.Write(playCount);
		writer.Write(freeSweepDailyCount);
		writer.Write(changedInventorySlot);
		writer.Write(rewardGold);
		writer.Write(gold);
		writer.Write(maxGold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		stamina = reader.ReadInt32();
		playCount = reader.ReadInt32();
		freeSweepDailyCount = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		rewardGold = reader.ReadInt64();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
	}
}
