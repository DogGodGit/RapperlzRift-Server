namespace ClientCommon;

public class RestRewardReceiveDiaResponseBody : ResponseBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
