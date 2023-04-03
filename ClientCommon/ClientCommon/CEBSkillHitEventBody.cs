using System;

namespace ClientCommon;

public class CEBSkillHitEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public Guid heroId;

	public int skillId;

	public int chainSkillId;

	public int hitId;

	public PDHitTarget[] targets;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(chainSkillId);
		writer.Write(hitId);
		writer.Write(targets);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		chainSkillId = reader.ReadInt32();
		hitId = reader.ReadInt32();
		targets = reader.ReadPDHitTargets();
	}
}
