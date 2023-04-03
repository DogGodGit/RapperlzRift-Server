using System;

namespace ClientCommon;

public class RankRewardReceiveResponseBody : ResponseBody
{
	public DateTime date;

	public long gold;

	public long maxGold;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
