using System;

namespace ClientCommon;

public class CEBSkillCastEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public Guid heroId;

	public int skillId;

	public int chainSkillId;

	public PDVector3 skillTargetPosition;

	public Guid targetHeroId;

	public PDVector3 heroTargetPosition;

	public float heroTargetRotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(chainSkillId);
		writer.Write(skillTargetPosition);
		writer.Write(targetHeroId);
		writer.Write(heroTargetPosition);
		writer.Write(heroTargetRotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		chainSkillId = reader.ReadInt32();
		skillTargetPosition = reader.ReadPDVector3();
		targetHeroId = reader.ReadGuid();
		heroTargetPosition = reader.ReadPDVector3();
		heroTargetRotationY = reader.ReadSingle();
	}
}
