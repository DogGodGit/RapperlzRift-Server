namespace ClientCommon;

public class CashProductPurchaseStartCommandBody : CommandBody
{
	public int productId;

	public int storeType;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(productId);
		writer.Write(storeType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		productId = reader.ReadInt32();
		storeType = reader.ReadInt32();
	}
}
