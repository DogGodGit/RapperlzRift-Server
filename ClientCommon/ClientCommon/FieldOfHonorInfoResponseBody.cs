using System;

namespace ClientCommon;

public class FieldOfHonorInfoResponseBody : ResponseBody
{
	public DateTime date;

	public int myRanking;

	public int successiveCount;

	public int playCount;

	public PDFieldOfHonorHistory[] histories;

	public PDFieldOfHonorHero[] matchedRankings;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(myRanking);
		writer.Write(successiveCount);
		writer.Write(playCount);
		writer.Write(histories);
		writer.Write(matchedRankings);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		myRanking = reader.ReadInt32();
		successiveCount = reader.ReadInt32();
		playCount = reader.ReadInt32();
		histories = reader.ReadPDPacketDatas<PDFieldOfHonorHistory>();
		matchedRankings = reader.ReadPDPacketDatas<PDFieldOfHonorHero>();
	}
}
