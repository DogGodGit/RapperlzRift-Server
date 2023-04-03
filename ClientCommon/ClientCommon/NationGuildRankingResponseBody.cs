namespace ClientCommon;

public class NationGuildRankingResponseBody : ResponseBody
{
	public PDGuildRanking myGuildRanking;

	public PDGuildRanking[] guildRankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myGuildRanking);
		writer.Write(guildRankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myGuildRanking = reader.ReadPDPacketData<PDGuildRanking>();
		guildRankings = reader.ReadPDPacketDatas<PDGuildRanking>();
	}
}
