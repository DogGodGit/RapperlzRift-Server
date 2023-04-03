namespace ClientCommon;

public class MainQuestCompleteResponseBody : ResponseBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public long gold;

	public long maxGold;

	public int rankNo;

	public PDFullHeroMainGear[] rewardMainGears;

	public int maxAcquisitionMainGearGrade;

	public PDFullHeroSubGear[] rewardSubGears;

	public PDInventorySlot[] changedInventorySlots;

	public PDHeroMount[] rewardMounts;

	public PDHeroCreatureCard[] changedCreatureCards;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(rankNo);
		writer.Write(rewardMainGears);
		writer.Write(maxAcquisitionMainGearGrade);
		writer.Write(rewardSubGears);
		writer.Write(changedInventorySlots);
		writer.Write(rewardMounts);
		writer.Write(changedCreatureCards);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		rankNo = reader.ReadInt32();
		rewardMainGears = reader.ReadPDPacketDatas<PDFullHeroMainGear>();
		maxAcquisitionMainGearGrade = reader.ReadInt32();
		rewardSubGears = reader.ReadPDPacketDatas<PDFullHeroSubGear>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		rewardMounts = reader.ReadPDPacketDatas<PDHeroMount>();
		changedCreatureCards = reader.ReadPDPacketDatas<PDHeroCreatureCard>();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
