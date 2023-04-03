using System;

namespace ClientCommon;

public class SEBGuildMissionCompletedEventBody : SEBServerEventBody
{
	public DateTime date;

	public PDHeroGuildMission nextMission;

	public int completedMissionCount;

	public long acquiredExp;

	public int maxHP;

	public int hp;

	public long exp;

	public int level;

	public int totalGuildContributionPoint;

	public int guildContributionPoint;

	public PDInventorySlot[] changedInventorySlots;

	public long giFund;

	public int giBuildingPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(nextMission);
		writer.Write(completedMissionCount);
		writer.Write(acquiredExp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(exp);
		writer.Write(level);
		writer.Write(totalGuildContributionPoint);
		writer.Write(guildContributionPoint);
		writer.Write(changedInventorySlots);
		writer.Write(giFund);
		writer.Write(giBuildingPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		nextMission = reader.ReadPDPacketData<PDHeroGuildMission>();
		completedMissionCount = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		exp = reader.ReadInt64();
		level = reader.ReadInt32();
		totalGuildContributionPoint = reader.ReadInt32();
		guildContributionPoint = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		giFund = reader.ReadInt64();
		giBuildingPoint = reader.ReadInt32();
	}
}
