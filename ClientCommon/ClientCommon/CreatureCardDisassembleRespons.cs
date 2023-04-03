namespace ClientCommon;

public class CreatureCardDisassembleResponseBody : ResponseBody
{
	public int soulPowder;

	public PDHeroCreatureCard changedCreatureCard;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(soulPowder);
		writer.Write(changedCreatureCard);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		soulPowder = reader.ReadInt32();
		changedCreatureCard = reader.ReadPDPacketData<PDHeroCreatureCard>();
	}
}
