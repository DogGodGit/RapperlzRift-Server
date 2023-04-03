namespace ClientCommon;

public class GoldItemUseResponseBody : ResponseBody
{
	public long gold;

	public long maxGold;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
