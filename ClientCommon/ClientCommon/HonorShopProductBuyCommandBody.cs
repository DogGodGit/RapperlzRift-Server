namespace ClientCommon;

public class HonorShopProductBuyCommandBody : CommandBody
{
	public int productId;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(productId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		productId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
