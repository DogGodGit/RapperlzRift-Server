using System;

namespace ClientCommon;

public class SecretLetterQuestAcceptResponseBody : ResponseBody
{
	public PDHeroSecretLetterQuest quest;

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
		quest = reader.ReadPDPacketData<PDHeroSecretLetterQuest>();
		date = reader.ReadDateTime();
		dailyStartCount = reader.ReadInt32();
	}
}
