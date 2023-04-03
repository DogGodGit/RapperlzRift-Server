namespace ClientCommon;

public class SEBFieldOfHonorClearEventBody : SEBServerEventBody
{
	public int myRanking;

	public int successiveCount;

	public int honorPoint;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(myRanking);
		writer.Write(successiveCount);
		writer.Write(honorPoint);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		myRanking = reader.ReadInt32();
		successiveCount = reader.ReadInt32();
		honorPoint = reader.ReadInt32();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
