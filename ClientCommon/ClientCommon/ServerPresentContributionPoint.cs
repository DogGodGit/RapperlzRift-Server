namespace ClientCommon;

public class ServerPresentContributionPointRankingCommandBody : CommandBody
{
}
public class ServerPresentContributionPointRankingResponseBody : ResponseBody
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
