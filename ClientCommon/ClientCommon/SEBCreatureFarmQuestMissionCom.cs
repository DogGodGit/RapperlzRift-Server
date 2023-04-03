namespace ClientCommon;

public class SEBCreatureFarmQuestMissionCompletedEventBody : SEBServerEventBody
{
	public long acquiredExp;

	public int maxHP;

	public int hp;

	public int level;

	public long exp;

	public int nextMissionNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(nextMissionNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		nextMissionNo = reader.ReadInt32();
	}
}
