namespace ClientCommon;

public class SEBFishingCastingCompletedEventBody : SEBServerEventBody
{
	public int castingCount;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(castingCount);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		castingCount = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
