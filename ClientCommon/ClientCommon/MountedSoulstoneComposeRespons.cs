namespace ClientCommon;

public class MountedSoulstoneComposeResponseBody : ResponseBody
{
	public PDInventorySlot[] changedInventorySlots;

	public long gold;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlots);
		writer.Write(gold);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		gold = reader.ReadInt64();
		maxHp = reader.ReadInt32();
	}
}
