namespace ClientCommon;

public class WingEnchantCommandBody : CommandBody
{
	public int wingPartId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wingPartId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wingPartId = reader.ReadInt32();
	}
}
