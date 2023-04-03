namespace ClientCommon;

public class FieldOfHonorTopRankingListResponseBody : ResponseBody
{
	public int myRanking;

	public PDFieldOfHonorRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadInt32();
		rankings = reader.ReadPDPacketDatas<PDFieldOfHonorRanking>();
	}
}
