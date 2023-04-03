namespace ClientCommon;

public class SupplySupportQuestCartChangeResponseBody : ResponseBody
{
	public int cartId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartId = reader.ReadInt32();
	}
}
