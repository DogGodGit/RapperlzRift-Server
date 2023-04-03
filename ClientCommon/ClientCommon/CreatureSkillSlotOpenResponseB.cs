namespace ClientCommon;

public class CreatureSkillSlotOpenResponseBody : ResponseBody
{
	public int additionalOpenSkillSlotCount;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(additionalOpenSkillSlotCount);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		additionalOpenSkillSlotCount = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
