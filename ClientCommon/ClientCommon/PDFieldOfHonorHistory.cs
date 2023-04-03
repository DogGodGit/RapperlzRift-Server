using System;

namespace ClientCommon;

public class PDFieldOfHonorHistory : PDPacketData
{
	public bool isChallenged;

	public int oldRanking;

	public int ranking;

	public bool isWin;

	public Guid targetHeroId;

	public string targetHeroName;

	public DateTimeOffset regTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isChallenged);
		writer.Write(oldRanking);
		writer.Write(ranking);
		writer.Write(isWin);
		writer.Write(targetHeroId);
		writer.Write(targetHeroName);
		writer.Write(regTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isChallenged = reader.ReadBoolean();
		oldRanking = reader.ReadInt32();
		ranking = reader.ReadInt32();
		isWin = reader.ReadBoolean();
		targetHeroId = reader.ReadGuid();
		targetHeroName = reader.ReadString();
		regTime = reader.ReadDateTimeOffset();
	}
}
