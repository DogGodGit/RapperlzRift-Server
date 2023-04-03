namespace ClientCommon;

public class SEBGuildDailyObjectiveNoticeEventBody : SEBServerEventBody
{
	public PDSimpleHero hero;

	public int contentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hero);
		writer.Write(contentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hero = reader.ReadPDPacketData<PDSimpleHero>();
		contentId = reader.ReadInt32();
	}
}
