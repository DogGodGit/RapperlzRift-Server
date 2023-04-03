namespace ClientCommon;

public class SEBCartChangedEventBody : SEBServerEventBody
{
	public long instanceId;

	public int cartId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(cartId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		cartId = reader.ReadInt32();
	}
}
