namespace ClientCommon;

public class DailyQuestAbandonResponseBody : ResponseBody
{
	public PDHeroDailyQuest addedDailyQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(addedDailyQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		addedDailyQuest = reader.ReadPDPacketData<PDHeroDailyQuest>();
	}
}
