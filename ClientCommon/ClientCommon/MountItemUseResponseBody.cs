namespace ClientCommon;

public class MountItemUseResponseBody : ResponseBody
{
	public int maxHP;

	public PDHeroMount addedMount;

	public PDInventorySlot changedInventorySlot;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(addedMount);
		writer.Write(changedInventorySlot);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		addedMount = reader.ReadPDPacketData<PDHeroMount>();
		changedInventorySlot = reader.ReadPDPacketData<PDInventorySlot>();
	}
}
