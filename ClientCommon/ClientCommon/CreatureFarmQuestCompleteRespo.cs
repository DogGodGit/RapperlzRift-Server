namespace ClientCommon;

public class CreatureFarmQuestCompleteResponseBody : ResponseBody
{
	public long acquiredExp;

	public int maxHP;

	public int hp;

	public int level;

	public long exp;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
