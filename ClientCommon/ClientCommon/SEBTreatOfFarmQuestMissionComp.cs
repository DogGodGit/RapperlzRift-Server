namespace ClientCommon;

public class SEBTreatOfFarmQuestMissionCompleteEventBody : SEBServerEventBody
{
	public int completedMissionCount;

	public PDHeroTreatOfFarmQuestMission nextMission;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(completedMissionCount);
		writer.Write(nextMission);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		completedMissionCount = reader.ReadInt32();
		nextMission = reader.ReadPDPacketData<PDHeroTreatOfFarmQuestMission>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
