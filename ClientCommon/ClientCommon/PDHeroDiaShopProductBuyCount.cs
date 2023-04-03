namespace ClientCommon;

public class PDHeroDiaShopProductBuyCount : PDPacketData
{
	public int productId;

	public int buyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(productId);
		writer.Write(buyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		productId = reader.ReadInt32();
		buyCount = reader.ReadInt32();
	}
}
