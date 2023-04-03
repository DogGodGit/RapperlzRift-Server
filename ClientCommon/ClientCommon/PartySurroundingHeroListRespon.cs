namespace ClientCommon;

public class PartySurroundingHeroListResponseBody : ResponseBody
{
	public PDSimpleHero[] heroes;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroes);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroes = reader.ReadPDPacketDatas<PDSimpleHero>();
	}
}
