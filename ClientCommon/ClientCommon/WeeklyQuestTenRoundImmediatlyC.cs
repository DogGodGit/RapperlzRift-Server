using System;

namespace ClientCommon;

public class WeeklyQuestTenRoundImmediatlyCompleteCommandBody : CommandBody
{
	public Guid roundId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(roundId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		roundId = reader.ReadGuid();
	}
}
public class WeeklyQuestTenRoundImmediatlyCompleteResponseBody : ResponseBody
{
	public long gold;

	public long maxGold;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public PDInventorySlot[] changedInventorySlots;

	public int nextRoundNo;

	public Guid nextRoundId;

	public int nextRoundMissionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(changedInventorySlots);
		writer.Write(nextRoundNo);
		writer.Write(nextRoundId);
		writer.Write(nextRoundMissionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		nextRoundNo = reader.ReadInt32();
		nextRoundId = reader.ReadGuid();
		nextRoundMissionId = reader.ReadInt32();
	}
}
