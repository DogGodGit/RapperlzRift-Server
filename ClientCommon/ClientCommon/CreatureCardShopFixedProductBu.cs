namespace ClientCommon;

public class CreatureCardShopFixedProductBuyCommandBody : CommandBody
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
public class CreatureCardShopFixedProductBuyResponseBody : ResponseBody
{
	public int soulPowder;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(soulPowder);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		soulPowder = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
