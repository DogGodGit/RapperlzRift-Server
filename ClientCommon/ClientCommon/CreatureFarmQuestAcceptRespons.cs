namespace ClientCommon;

public class CreatureFarmQuestAcceptResponseBody : ResponseBody
{
	public PDHeroCreatureFarmQuest heroCreatureFarmQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroCreatureFarmQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroCreatureFarmQuest = reader.ReadPDPacketData<PDHeroCreatureFarmQuest>();
	}
}
