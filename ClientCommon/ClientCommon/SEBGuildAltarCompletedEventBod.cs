namespace ClientCommon;

public class SEBGuildAltarCompletedEventBody : SEBServerEventBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public int contributionPoint;

	public int totalContributionPoint;

	public long giFund;

	public int giBuildingPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(contributionPoint);
		writer.Write(totalContributionPoint);
		writer.Write(giFund);
		writer.Write(giBuildingPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		contributionPoint = reader.ReadInt32();
		totalContributionPoint = reader.ReadInt32();
		giFund = reader.ReadInt64();
		giBuildingPoint = reader.ReadInt32();
	}
}
