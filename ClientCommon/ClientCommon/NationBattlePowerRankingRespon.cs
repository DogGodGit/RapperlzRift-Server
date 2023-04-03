namespace ClientCommon;

public class NationBattlePowerRankingResponseBody : ResponseBody
{
	public PDRanking myRanking;

	public PDRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadPDPacketData<PDRanking>();
		rankings = reader.ReadPDPacketDatas<PDRanking>();
	}
}
