using System;

namespace ClientCommon;

public class SupplySupportQuestCompleteResponseBody : ResponseBody
{
	public long acquiredExp;

	public int acquiredExploitPoint;

	public int level;

	public long exp;

	public int exploitPoint;

	public int maxHp;

	public int hp;

	public long gold;

	public long maxGold;

	public DateTime date;

	public int dailyExploitPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(acquiredExploitPoint);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(exploitPoint);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(date);
		writer.Write(dailyExploitPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		acquiredExploitPoint = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		exploitPoint = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		date = reader.ReadDateTime();
		dailyExploitPoint = reader.ReadInt32();
	}
}
