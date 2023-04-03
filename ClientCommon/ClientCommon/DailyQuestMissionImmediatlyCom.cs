using System;

namespace ClientCommon;

public class DailyQuestMissionImmediatlyCompleteCommandBody : CommandBody
{
	public Guid questId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadGuid();
	}
}
public class DailyQuestMissionImmediatlyCompleteResponseBody : ResponseBody
{
	public long gold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
	}
}
