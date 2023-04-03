namespace ClientCommon;

public class TrueHeroQuestAcceptResponseBody : ResponseBody
{
	public PDHeroTrueHeroQuest trueHeroQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(trueHeroQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		trueHeroQuest = reader.ReadPDPacketData<PDHeroTrueHeroQuest>();
	}
}
