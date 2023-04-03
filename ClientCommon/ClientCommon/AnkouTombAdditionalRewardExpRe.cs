namespace ClientCommon;

public class AnkouTombAdditionalRewardExpReceiveCommandBody : CommandBody
{
	public int rewardExpType;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rewardExpType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rewardExpType = reader.ReadInt32();
	}
}
public class AnkouTombAdditionalRewardExpReceiveResponseBody : ResponseBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
