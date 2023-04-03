using System;

namespace ClientCommon;

public class DailyQuestAcceptResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyQuestAcceptionCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyQuestAcceptionCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyQuestAcceptionCount = reader.ReadInt32();
	}
}
