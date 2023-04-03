namespace ClientCommon;

public class ServerIllustratedBookRankingResponseBody : ResponseBody
{
	public PDIllustratedBookRanking myRanking;

	public PDIllustratedBookRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadPDPacketData<PDIllustratedBookRanking>();
		rankings = reader.ReadPDPacketDatas<PDIllustratedBookRanking>();
	}
}
