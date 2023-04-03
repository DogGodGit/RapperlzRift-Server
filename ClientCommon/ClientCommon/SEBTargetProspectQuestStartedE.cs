namespace ClientCommon;

public class SEBTargetProspectQuestStartedEventBody : SEBServerEventBody
{
	public PDHeroProspectQuest quest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quest = reader.ReadPDPacketData<PDHeroProspectQuest>();
	}
}
