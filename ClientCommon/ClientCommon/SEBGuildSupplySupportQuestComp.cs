namespace ClientCommon;

public class SEBGuildSupplySupportQuestCompletedEventBody : SEBServerEventBody
{
	public long acquiredExp;

	public int maxHP;

	public int hp;

	public long exp;

	public int level;

	public int totalGuildContributionPoint;

	public int guildContributionPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(exp);
		writer.Write(level);
		writer.Write(totalGuildContributionPoint);
		writer.Write(guildContributionPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		exp = reader.ReadInt64();
		level = reader.ReadInt32();
		totalGuildContributionPoint = reader.ReadInt32();
		guildContributionPoint = reader.ReadInt32();
	}
}
