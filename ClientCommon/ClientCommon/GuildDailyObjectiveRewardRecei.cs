using System;

namespace ClientCommon;

public class GuildDailyObjectiveRewardReceiveCommandBody : CommandBody
{
}
public class GuildDailyObjectiveRewardReceiveResponseBody : ResponseBody
{
	public DateTime date;

	public int rewardNo;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(rewardNo);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		rewardNo = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
