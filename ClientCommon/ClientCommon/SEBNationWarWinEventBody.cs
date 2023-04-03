using System;

namespace ClientCommon;

public class SEBNationWarWinEventBody : SEBServerEventBody
{
	public PDItemBooty[] joinBooties;

	public long joinAcquiredExp;

	public int joinAcquiredExploitPoint;

	public int objectiveAchievementOwnDia;

	public int objectiveAchievementAcquiredExploitPoint;

	public PDItemBooty rankingBooty;

	public PDItemBooty luckyBooty;

	public int level;

	public long exp;

	public int exploitPoint;

	public int maxHp;

	public int hp;

	public int ownDia;

	public DateTime date;

	public int dailyExploitPoint;

	public PDInventorySlot[] changedInventorySlots;

	public int accNationWarWinCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(joinBooties);
		writer.Write(joinAcquiredExp);
		writer.Write(joinAcquiredExploitPoint);
		writer.Write(objectiveAchievementOwnDia);
		writer.Write(objectiveAchievementAcquiredExploitPoint);
		writer.Write(rankingBooty);
		writer.Write(luckyBooty);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(exploitPoint);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(ownDia);
		writer.Write(date);
		writer.Write(dailyExploitPoint);
		writer.Write(changedInventorySlots);
		writer.Write(accNationWarWinCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		joinBooties = reader.ReadPDBooties<PDItemBooty>();
		joinAcquiredExp = reader.ReadInt64();
		joinAcquiredExploitPoint = reader.ReadInt32();
		objectiveAchievementOwnDia = reader.ReadInt32();
		objectiveAchievementAcquiredExploitPoint = reader.ReadInt32();
		rankingBooty = reader.ReadPDBooty<PDItemBooty>();
		luckyBooty = reader.ReadPDBooty<PDItemBooty>();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		exploitPoint = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		date = reader.ReadDateTime();
		dailyExploitPoint = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		accNationWarWinCount = reader.ReadInt32();
	}
}
