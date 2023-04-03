namespace ClientCommon;

public class JobChangeQuestAcceptResponseBody : ResponseBody
{
	public PDHeroJobChangeQuest quest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(quest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		quest = reader.ReadPDPacketData<PDHeroJobChangeQuest>();
	}
}
