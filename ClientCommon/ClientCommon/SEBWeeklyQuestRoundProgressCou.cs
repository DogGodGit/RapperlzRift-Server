using System;

namespace ClientCommon;

public class SEBWeeklyQuestRoundProgressCountUpdatedEventBody : SEBServerEventBody
{
	public Guid roundId;

	public int roundProgressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(roundId);
		writer.Write(roundProgressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		roundId = reader.ReadGuid();
		roundProgressCount = reader.ReadInt32();
	}
}
