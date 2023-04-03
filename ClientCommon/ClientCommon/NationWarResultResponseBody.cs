namespace ClientCommon;

public class NationWarResultResponseBody : ResponseBody
{
	public int winNationId;

	public int myRanking;

	public int myKillCount;

	public int myAssistCount;

	public int myDeadCount;

	public int myImmediateRevivalCount;

	public long rewardedExp;

	public int offenseNationId;

	public PDNationWarRanking[] offenseNationRankings;

	public int defenseNationId;

	public PDNationWarRanking[] defenseNationRankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(winNationId);
		writer.Write(myRanking);
		writer.Write(myKillCount);
		writer.Write(myAssistCount);
		writer.Write(myDeadCount);
		writer.Write(myImmediateRevivalCount);
		writer.Write(rewardedExp);
		writer.Write(offenseNationId);
		writer.Write(offenseNationRankings);
		writer.Write(defenseNationId);
		writer.Write(defenseNationRankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		winNationId = reader.ReadInt32();
		myRanking = reader.ReadInt32();
		myKillCount = reader.ReadInt32();
		myAssistCount = reader.ReadInt32();
		myDeadCount = reader.ReadInt32();
		myImmediateRevivalCount = reader.ReadInt32();
		rewardedExp = reader.ReadInt64();
		offenseNationId = reader.ReadInt32();
		offenseNationRankings = reader.ReadPDPacketDatas<PDNationWarRanking>();
		defenseNationId = reader.ReadInt32();
		defenseNationRankings = reader.ReadPDPacketDatas<PDNationWarRanking>();
	}
}
