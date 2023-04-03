namespace ClientCommon;

public class SEBHeroEnterEventBody : SEBServerEventBody
{
	public PDHero hero;

	public bool isRevivalEnter;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hero);
		writer.Write(isRevivalEnter);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hero = reader.ReadPDPacketData<PDHero>();
		isRevivalEnter = reader.ReadBoolean();
	}
}
