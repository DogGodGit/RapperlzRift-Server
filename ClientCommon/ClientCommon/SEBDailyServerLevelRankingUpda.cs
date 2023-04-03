namespace ClientCommon;

public class SEBDailyServerLevelRankingUpdatedEventBody : SEBServerEventBody
{
	public int rankingNo;

	public int ranking;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rankingNo);
		writer.Write(ranking);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rankingNo = reader.ReadInt32();
		ranking = reader.ReadInt32();
	}
}
