namespace ClientCommon;

public class SubGearQualityUpResponseBody : ResponseBody
{
	public int subGearQuality;

	public PDInventorySlot[] changedInventorySlots;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearQuality);
		writer.Write(changedInventorySlots);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearQuality = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		maxHp = reader.ReadInt32();
	}
}
