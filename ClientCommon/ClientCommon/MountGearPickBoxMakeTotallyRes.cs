namespace ClientCommon;

public class MountGearPickBoxMakeTotallyResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public long gold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(gold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		gold = reader.ReadInt64();
	}
}
