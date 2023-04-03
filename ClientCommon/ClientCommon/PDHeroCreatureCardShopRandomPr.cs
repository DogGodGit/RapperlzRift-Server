namespace ClientCommon;

public class PDHeroCreatureCardShopRandomProduct : PDPacketData
{
	public int productId;

	public bool purchased;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(productId);
		writer.Write(purchased);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		productId = reader.ReadInt32();
		purchased = reader.ReadBoolean();
	}
}
