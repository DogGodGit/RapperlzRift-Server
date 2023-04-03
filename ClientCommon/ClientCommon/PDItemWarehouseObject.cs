namespace ClientCommon;

public class PDItemWarehouseObject : PDWarehouseObject
{
	public int itemId;

	public bool owned;

	public int count;

	public override int type => 3;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(itemId);
		writer.Write(owned);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		itemId = reader.ReadInt32();
		owned = reader.ReadBoolean();
		count = reader.ReadInt32();
	}
}
