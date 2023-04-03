namespace ClientCommon;

public class PDItemLuckyShopPickResult : PDPacketData
{
	public int itemId;

	public int count;

	public PDItemLuckyShopPickResult()
	{
	}

	public PDItemLuckyShopPickResult(int itemId, int count)
	{
		this.itemId = itemId;
		this.count = count;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(itemId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		itemId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
