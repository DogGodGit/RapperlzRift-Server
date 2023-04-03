namespace ClientCommon;

public class ServerCreatureCardRankingResponseBody : ResponseBody
{
	public PDCreatureCardRanking myRanking;

	public PDCreatureCardRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadPDPacketData<PDCreatureCardRanking>();
		rankings = reader.ReadPDPacketDatas<PDCreatureCardRanking>();
	}
}
