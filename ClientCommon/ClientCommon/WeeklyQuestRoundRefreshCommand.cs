using System;

namespace ClientCommon;

public class WeeklyQuestRoundRefreshCommandBody : CommandBody
{
	public Guid roundId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(roundId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		roundId = reader.ReadGuid();
	}
}
