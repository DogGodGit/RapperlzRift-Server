using System;

namespace ClientCommon;

public class WeeklyQuestRoundRefreshResponseBody : ResponseBody
{
	public long gold;

	public Guid newRoundId;

	public int newRoundMissionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(newRoundId);
		writer.Write(newRoundMissionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		newRoundId = reader.ReadGuid();
		newRoundMissionId = reader.ReadInt32();
	}
}
