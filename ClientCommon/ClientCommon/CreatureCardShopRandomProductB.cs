namespace ClientCommon;

public class CreatureCardShopRandomProductBuyCommandBody : CommandBody
{
	public int productId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(productId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		productId = reader.ReadInt32();
	}
}
public class CreatureCardShopRandomProductBuyResponseBody : ResponseBody
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
