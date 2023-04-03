namespace ClientCommon;

public class Open7DayEventProductBuyCommandBody : CommandBody
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
