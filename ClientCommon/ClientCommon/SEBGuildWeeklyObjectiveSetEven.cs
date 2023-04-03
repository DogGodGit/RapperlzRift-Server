using System;

namespace ClientCommon;

public class SEBGuildWeeklyObjectiveSetEventBody : SEBServerEventBody
{
	public DateTime date;

	public int objectiveId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(objectiveId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		objectiveId = reader.ReadInt32();
	}
}
