namespace ClientCommon;

public class GuildFoodWarehouseStockCommandBody : CommandBody
{
	public int itemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(itemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		itemId = reader.ReadInt32();
	}
}
