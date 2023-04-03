namespace ClientCommon;

public class SkillLevelUpTotallyResponseBody : ResponseBody
{
	public int skillLevel;

	public PDInventorySlot[] changedInventorySlots;

	public long gold;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(skillLevel);
		writer.Write(changedInventorySlots);
		writer.Write(gold);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		skillLevel = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		gold = reader.ReadInt64();
	}
}
