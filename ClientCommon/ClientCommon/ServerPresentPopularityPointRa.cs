namespace ClientCommon;

public class ServerPresentPopularityPointRankingCommandBody : CommandBody
{
}
public class ServerPresentPopularityPointRankingResponseBody : ResponseBody
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
