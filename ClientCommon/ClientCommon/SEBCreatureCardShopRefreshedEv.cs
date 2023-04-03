namespace ClientCommon;

public class SEBCreatureCardShopRefreshedEventBody : SEBServerEventBody
{
	public PDHeroCreatureCardShopRandomProduct[] randomProducts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(randomProducts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		randomProducts = reader.ReadPDPacketDatas<PDHeroCreatureCardShopRandomProduct>();
	}
}
