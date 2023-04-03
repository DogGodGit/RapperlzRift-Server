namespace ClientCommon;

public class CreatureCardComposeResponseBody : ResponseBody
{
	public PDHeroCreatureCard changedCreatureCard;

	public int soulPowder;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedCreatureCard);
		writer.Write(soulPowder);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedCreatureCard = reader.ReadPDPacketData<PDHeroCreatureCard>();
		soulPowder = reader.ReadInt32();
	}
}
