namespace ClientCommon;

public class NationWeeklyPresentPopularityPointRankingRewardReceiveCommandBody : CommandBody
{
}
public class NationWeeklyPresentPopularityPointRankingRewardReceiveResponseBody : ResponseBody
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
public class NationWeeklyPresentPopularityPointRankingCommandBody : CommandBody
{
}
public class NationWeeklyPresentPopularityPointRankingResponseBody : ResponseBody
{
	public PDPresentPopularityPointRanking myRanking;

	public PDPresentPopularityPointRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadPDPacketData<PDPresentPopularityPointRanking>();
		rankings = reader.ReadPDPacketDatas<PDPresentPopularityPointRanking>();
	}
}
