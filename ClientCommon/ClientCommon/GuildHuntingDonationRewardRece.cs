using System;

namespace ClientCommon;

public class GuildHuntingDonationRewardReceiveCommandBody : CommandBody
{
}
public class GuildHuntingDonationRewardReceiveResponseBody : ResponseBody
{
	public DateTime date;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
