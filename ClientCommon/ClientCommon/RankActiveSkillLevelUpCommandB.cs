namespace ClientCommon;

public class RankActiveSkillLevelUpCommandBody : CommandBody
{
	public int targetSkillId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetSkillId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetSkillId = reader.ReadInt32();
	}
}
