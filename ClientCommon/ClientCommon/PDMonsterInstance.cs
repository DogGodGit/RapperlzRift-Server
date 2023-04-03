using System;

namespace ClientCommon;

public abstract class PDMonsterInstance : PDPacketData
{
	public long instanceId;

	public int monsterId;

	public int maxHP;

	public int hp;

	public int maxMentalStrength;

	public int mentalStrength;

	public PDAbnormalStateEffect[] abnormalStateEffects;

	public PDVector3 spawnedPosition;

	public float spawnedRotationY;

	public PDVector3 position;

	public float rotationY;

	public Guid ownerId;

	public MonsterOwnerType ownerType;

	public bool isExclusive;

	public Guid exclusiveHeroId;

	public string exclusiveHeroName;

	public int nationId;

	public bool isReturnMode;

	public Guid tamerId;

	public abstract MonsterInstanceType type { get; }

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.WriteEnumInt(type);
		writer.Write(instanceId);
		writer.Write(monsterId);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(maxMentalStrength);
		writer.Write(mentalStrength);
		writer.Write(abnormalStateEffects);
		writer.Write(spawnedPosition);
		writer.Write(spawnedRotationY);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(ownerId);
		writer.WriteEnumInt(ownerType);
		writer.Write(isExclusive);
		writer.Write(exclusiveHeroId);
		writer.Write(exclusiveHeroName);
		writer.Write(nationId);
		writer.Write(isReturnMode);
		writer.Write(tamerId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		monsterId = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		maxMentalStrength = reader.ReadInt32();
		mentalStrength = reader.ReadInt32();
		abnormalStateEffects = reader.ReadPDPacketDatas<PDAbnormalStateEffect>();
		spawnedPosition = reader.ReadPDVector3();
		spawnedRotationY = reader.ReadSingle();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		ownerId = reader.ReadGuid();
		ownerType = reader.ReadEnumInt<MonsterOwnerType>();
		isExclusive = reader.ReadBoolean();
		exclusiveHeroId = reader.ReadGuid();
		exclusiveHeroName = reader.ReadString();
		nationId = reader.ReadInt32();
		isReturnMode = reader.ReadBoolean();
		tamerId = reader.ReadGuid();
	}

	public static PDMonsterInstance Create(MonsterInstanceType type)
	{
		return type switch
		{
			MonsterInstanceType.ContinentMonster => new PDContinentMonsterInstance(), 
			MonsterInstanceType.MainQuestDungeonMonster => new PDMainQuestDungeonMonsterInstance(), 
			MonsterInstanceType.MainQuestDungeonSummonMonster => new PDMainQuestDungeonSummonMonsterInstance(), 
			MonsterInstanceType.StoryDungeonMonster => new PDStoryDungeonMonsterInstance(), 
			MonsterInstanceType.ExpDungeonMonster => new PDExpDungeonMonsterInstance(), 
			MonsterInstanceType.ExpDungeonLakChargeMonster => new PDExpDungeonLakChargeMonsterInstance(), 
			MonsterInstanceType.GoldDungeonMonster => new PDGoldDungeonMonsterInstance(), 
			MonsterInstanceType.TreatOfFarmQuestMonster => new PDTreatOfFarmQuestMonsterInstance(), 
			MonsterInstanceType.UndergroundMazeMonster => new PDUndergroundMazeMonsterInstance(), 
			MonsterInstanceType.ArtifactRoomMonster => new PDArtifactRoomMonsterInstance(), 
			MonsterInstanceType.AncientRelicNormalMonster => new PDAncientRelicNormalMonsterInstance(), 
			MonsterInstanceType.AncientRelicBossMonster => new PDAncientRelicBossMonsterInstance(), 
			MonsterInstanceType.GuildMissionQuestMonster => new PDGuildMissionQuestMonsterInstance(), 
			MonsterInstanceType.GuildAltarMonster => new PDGuildAltarMonsterInstance(), 
			MonsterInstanceType.NationWarMonster => new PDNationWarMonsterInstance(), 
			MonsterInstanceType.SoulCoveterMonster => new PDSoulCoveterMonsterInstance(), 
			MonsterInstanceType.ContinentEliteMonster => new PDContinentEliteMonsterInstance(), 
			MonsterInstanceType.EliteDungeonMonster => new PDEliteDungeonMonsterInstance(), 
			MonsterInstanceType.ProofOfValorBossMonster => new PDProofOfValorBossMonsterInstance(), 
			MonsterInstanceType.ProofOfValorNormalMonster => new PDProofOfValorNormalMonsterInstance(), 
			MonsterInstanceType.WisdomTempleColorMatchingMonster => new PDWisdomTempleColorMatchingMonsterInstance(), 
			MonsterInstanceType.WisdomTempleTreasureBoxMonster => new PDWisdomTempleTreasureBoxMonsterInstance(), 
			MonsterInstanceType.WisdomTempleQuizMonster => new PDWisdomTempleQuizMonsterInstance(), 
			MonsterInstanceType.WisdomTempleBossMonster => new PDWisdomTempleBossMonsterInstance(), 
			MonsterInstanceType.RuinsReclaimMonster => new PDRuinsReclaimMonsterInstance(), 
			MonsterInstanceType.RuinsReclaimSummonMonster => new PDRuinsReclaimSummonMonsterInstance(), 
			MonsterInstanceType.InfiniteWarMonster => new PDInfiniteWarMonsterInstance(), 
			MonsterInstanceType.FieldBossMonster => new PDFieldBossMonsterInstance(), 
			MonsterInstanceType.FearAltarMonster => new PDFearAltarMonsterInstance(), 
			MonsterInstanceType.FearAltarHalidomMonster => new PDFearAltarHalidomMonsterInstance(), 
			MonsterInstanceType.WarMemoryMonster => new PDWarMemoryMonsterInstance(), 
			MonsterInstanceType.WarMemorySummonMonster => new PDWarMemorySummonMonsterInstance(), 
			MonsterInstanceType.OsirisMonster => new PDOsirisRoomMonsterInstance(), 
			MonsterInstanceType.BiographQuestDungeonMonster => new PDBiographyQuestDungeonMonsterInstance(), 
			MonsterInstanceType.DragonNestMonster => new PDDragonNestMonsterInstance(), 
			MonsterInstanceType.CreatureFarmQuestMissionMonster => new PDCreatureFarmQuestMissionMonsterInstance(), 
			MonsterInstanceType.AnkouTombMonster => new PDAnkouTombMonsterInstance(), 
			MonsterInstanceType.JobChangeQuestMonster => new PDJobChangeQuestMonsterInstance(), 
			MonsterInstanceType.TradeShipMonster => new PDTradeShipMonsterInstance(), 
			MonsterInstanceType.TradeShipAdditionalMonster => new PDTradeShipAdditionalMonsterInstance(), 
			MonsterInstanceType.TradeShipObject => new PDTradeShipObjectInstance(), 
			_ => null, 
		};
	}
}
