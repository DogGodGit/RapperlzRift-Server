namespace ClientCommon;

public class SEBMainQuestDungeonStepCompletedEventBody : SEBServerEventBody
{
	public int stepNo;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public long gold;

	public long maxGold;

	public long acquiredExp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(gold);
		writer.Write(maxGold);
		writer.Write(acquiredExp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxGold = reader.ReadInt64();
		acquiredExp = reader.ReadInt64();
	}
}
