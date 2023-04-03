namespace ClientCommon;

public class NationWeeklyPresentContributionPointRankingRewardReceiveCommandBody : CommandBody
{
}
public class NationWeeklyPresentContributionPointRankingRewardReceiveResponseBody : ResponseBody
{
	public int rewardedRankingNo;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardedRankingNo);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardedRankingNo = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
public class NationWeeklyPresentContributionPointRankingCommandBody : CommandBody
{
}
public class NationWeeklyPresentContributionPointRankingResponseBody : ResponseBody
{
	public PDPresentContributionPointRanking myRanking;

	public PDPresentContributionPointRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadPDPacketData<PDPresentContributionPointRanking>();
		rankings = reader.ReadPDPacketDatas<PDPresentContributionPointRanking>();
	}
}
