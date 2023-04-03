namespace ClientCommon;

public class GuildSkillLevelUpCommandBody : CommandBody
{
	public int skillId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(skillId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		skillId = reader.ReadInt32();
	}
}
