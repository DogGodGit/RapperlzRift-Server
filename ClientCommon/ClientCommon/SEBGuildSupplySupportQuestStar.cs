using System;

namespace ClientCommon;

public class SEBGuildSupplySupportQuestStartedEventBody : SEBServerEventBody
{
	public DateTime date;

	public int dailyGuildSupplySupportQuestStartCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyGuildSupplySupportQuestStartCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyGuildSupplySupportQuestStartCount = reader.ReadInt32();
	}
}
