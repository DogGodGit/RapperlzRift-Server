namespace ClientCommon;

public class SEBDailyServerNationPowerRankingUpdatedEventBody : SEBServerEventBody
{
	public PDNationPowerRanking[] rankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rankings = reader.ReadPDPacketDatas<PDNationPowerRanking>();
	}
}
