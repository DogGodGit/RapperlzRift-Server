namespace ClientCommon;

public class DailyQuestCompleteResponseBody : ResponseBody
{
	public int vipPoint;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public PDInventorySlot[] changedInventorySlot;

	public PDHeroDailyQuest addedDailyQuest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(vipPoint);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(changedInventorySlot);
		writer.Write(addedDailyQuest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		vipPoint = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlot = reader.ReadPDPacketDatas<PDInventorySlot>();
		addedDailyQuest = reader.ReadPDPacketData<PDHeroDailyQuest>();
	}
}
