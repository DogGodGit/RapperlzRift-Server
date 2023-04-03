using System;

namespace ClientCommon;

public class HolyWarQuestAcceptResponseBody : ResponseBody
{
	public DateTime date;

	public PDHeroHolyWarQuest quest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(quest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		quest = reader.ReadPDPacketData<PDHeroHolyWarQuest>();
	}
}
