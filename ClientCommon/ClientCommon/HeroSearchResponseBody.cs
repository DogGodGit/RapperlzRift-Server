namespace ClientCommon;

public class HeroSearchResponseBody : ResponseBody
{
	public PDSearchHero[] heroes;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroes);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroes = reader.ReadPDPacketDatas<PDSearchHero>();
	}
}
