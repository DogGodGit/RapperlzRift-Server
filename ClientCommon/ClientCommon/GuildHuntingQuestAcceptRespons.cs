using System;

namespace ClientCommon;

public class GuildHuntingQuestAcceptResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyGuildHuntingQuestStartCount;

	public PDHeroGuildHuntingQuest guildHuntingQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyGuildHuntingQuestStartCount);
		writer.Write(guildHuntingQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyGuildHuntingQuestStartCount = reader.ReadInt32();
		guildHuntingQuest = reader.ReadPDPacketData<PDHeroGuildHuntingQuest>();
	}
}
