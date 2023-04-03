using System;

namespace ClientCommon;

public class CEBWarMemoryTransformationMonsterSkillHitEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public int skillId;

	public int hitId;

	public PDHitTarget[] targets;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(skillId);
		writer.Write(hitId);
		writer.Write(targets);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		hitId = reader.ReadInt32();
		targets = reader.ReadPDHitTargets();
	}
}
public class CEBWarMemoryTransformationMonsterSkillCastEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public int skillId;

	public PDVector3 skillTargetPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(skillId);
		writer.Write(skillTargetPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		skillTargetPosition = reader.ReadPDVector3();
	}
}
