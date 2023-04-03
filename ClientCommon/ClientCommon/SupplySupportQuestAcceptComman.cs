namespace ClientCommon;

public class SupplySupportQuestAcceptCommandBody : CommandBody
{
	public int orderId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(orderId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		orderId = reader.ReadInt32();
	}
}
