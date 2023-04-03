using System;

namespace ClientCommon;

public class SEBHeroMainQuestTransformationMonsterSkillCastEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int skillId;

	public PDVector3 skillTargetPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(skillTargetPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		skillTargetPosition = reader.ReadPDVector3();
	}
}
