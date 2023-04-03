namespace ClientCommon;

public class MountLevelUpResponseBody : ResponseBody
{
	public PDInventorySlot changedInventorySlot;

	public int mountLevel;

	public int mountSatiety;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(changedInventorySlot);
		writer.Write(mountLevel);
		writer.Write(mountSatiety);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
		mountLevel = reader.ReadInt32();
		mountSatiety = reader.ReadInt32();
		maxHp = reader.ReadInt32();
	}
}
