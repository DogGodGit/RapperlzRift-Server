using System;

namespace ClientCommon;

public class DailyQuestAbandonCommandBody : CommandBody
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
