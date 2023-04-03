namespace ClientCommon;

public class SEBHeroDailyQuestCreatedEventBody : SEBServerEventBody
{
	public PDHeroDailyQuest[] quests;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quests);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quests = reader.ReadPDPacketDatas<PDHeroDailyQuest>();
	}
}
