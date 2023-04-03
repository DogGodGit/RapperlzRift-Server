using System;

namespace ClientCommon;

public class GuildWeeklyObjectiveRewardReceiveCommandBody : CommandBody
{
}
public class GuildWeeklyObjectiveRewardReceiveResponseBody : ResponseBody
{
	public DateTime rewardReceivedDate;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardReceivedDate);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardReceivedDate = reader.ReadDateTime();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
