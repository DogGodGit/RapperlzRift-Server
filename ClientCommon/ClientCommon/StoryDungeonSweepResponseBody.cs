using System;

namespace ClientCommon;

public class StoryDungeonSweepResponseBody : ResponseBody
{
	public DateTime date;

	public int stamina;

	public int playCount;

	public int freeSweepDailyCount;

	public PDInventorySlot[] changedInventorySlots;

	public PDItemBooty[] booties;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(stamina);
		writer.Write(playCount);
		writer.Write(freeSweepDailyCount);
		writer.Write(changedInventorySlots);
		writer.Write(booties);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		stamina = reader.ReadInt32();
		playCount = reader.ReadInt32();
		freeSweepDailyCount = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		booties = reader.ReadPDBooties<PDItemBooty>();
	}
}
