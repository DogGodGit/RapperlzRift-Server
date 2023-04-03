using System;

namespace ClientCommon;

public class GuildHuntingDonateResponseBody : ResponseBody
{
	public DateTime date;

	public PDInventorySlot[] changedInventorySlots;

	public int guildHuntingDonationCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(changedInventorySlots);
		writer.Write(guildHuntingDonationCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		guildHuntingDonationCount = reader.ReadInt32();
	}
}
