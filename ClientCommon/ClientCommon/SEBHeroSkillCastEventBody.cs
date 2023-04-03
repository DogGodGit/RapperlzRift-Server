using System;

namespace ClientCommon;

public class SEBHeroSkillCastEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int skillId;

	public int chainSkillId;

	public PDVector3 heroTargetPosition;

	public PDVector3 skillTargetPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(chainSkillId);
		writer.Write(heroTargetPosition);
		writer.Write(skillTargetPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		chainSkillId = reader.ReadInt32();
		heroTargetPosition = reader.ReadPDVector3();
		skillTargetPosition = reader.ReadPDVector3();
	}
}
