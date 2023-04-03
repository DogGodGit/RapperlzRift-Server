namespace ClientCommon;

public class GuildSkillLevelUpResponseBody : ResponseBody
{
	public int level;

	public int contributionPoint;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(level);
		writer.Write(contributionPoint);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		level = reader.ReadInt32();
		contributionPoint = reader.ReadInt32();
		maxHp = reader.ReadInt32();
	}
}
