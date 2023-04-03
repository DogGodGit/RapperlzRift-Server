using System;

namespace ClientCommon;

public class SEBGuildDailyObjectiveSetEventBody : SEBServerEventBody
{
	public DateTime date;

	public int contentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(contentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		contentId = reader.ReadInt32();
	}
}
