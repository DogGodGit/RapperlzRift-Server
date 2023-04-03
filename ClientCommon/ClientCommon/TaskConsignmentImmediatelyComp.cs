using System;

namespace ClientCommon;

public class TaskConsignmentImmediatelyCompleteCommandBody : CommandBody
{
	public Guid instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
	}
}
public class TaskConsignmentImmediatelyCompleteResponseBody : ResponseBody
{
	public long gold;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public int totalGuildContributionPoint;

	public int guildContributionPoint;

	public PDInventorySlot[] changedInventorySlots;

	public long giFund;

	public int giBuildingPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(totalGuildContributionPoint);
		writer.Write(guildContributionPoint);
		writer.Write(changedInventorySlots);
		writer.Write(giFund);
		writer.Write(giBuildingPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		totalGuildContributionPoint = reader.ReadInt32();
		guildContributionPoint = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		giFund = reader.ReadInt64();
		giBuildingPoint = reader.ReadInt32();
	}
}
