namespace ClientCommon;

public class CreatureCardDisassembleAllResponseBody : ResponseBody
{
	public int soulPowder;

	public PDHeroCreatureCard[] changedCreatureCards;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(soulPowder);
		writer.Write(changedCreatureCards);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		soulPowder = reader.ReadInt32();
		changedCreatureCards = reader.ReadPDPacketDatas<PDHeroCreatureCard>();
	}
}
