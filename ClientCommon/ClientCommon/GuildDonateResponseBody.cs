using System;

namespace ClientCommon;

public class GuildDonateResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyDonationCount;

	public long gold;

	public int ownDia;

	public int unOwnDia;

	public int totalContributionPoint;

	public int contributionPoint;

	public long fund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyDonationCount);
		writer.Write(gold);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(totalContributionPoint);
		writer.Write(contributionPoint);
		writer.Write(fund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyDonationCount = reader.ReadInt32();
		gold = reader.ReadInt64();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		totalContributionPoint = reader.ReadInt32();
		contributionPoint = reader.ReadInt32();
		fund = reader.ReadInt64();
	}
}
