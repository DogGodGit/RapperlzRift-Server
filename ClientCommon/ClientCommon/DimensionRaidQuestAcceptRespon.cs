using System;

namespace ClientCommon;

public class DimensionRaidQuestAcceptResponseBody : ResponseBody
{
	public PDHeroDimensionRaidQuest quest;

	public DateTime date;

	public int dailyStartCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quest);
		writer.Write(date);
		writer.Write(dailyStartCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quest = reader.ReadPDPacketData<PDHeroDimensionRaidQuest>();
		date = reader.ReadDateTime();
		dailyStartCount = reader.ReadInt32();
	}
}
