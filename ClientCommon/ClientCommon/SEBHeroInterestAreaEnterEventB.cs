namespace ClientCommon;

public class SEBHeroInterestAreaEnterEventBody : SEBServerEventBody
{
	public PDHero hero;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hero);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hero = reader.ReadPDPacketData<PDHero>();
	}
}
