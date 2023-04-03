using System;

namespace ClientCommon;

public class CEBJobCommonSkillHitEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public int skillId;

	public int hitId;

	public long targetMonsterInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(skillId);
		writer.Write(hitId);
		writer.Write(targetMonsterInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		hitId = reader.ReadInt32();
		targetMonsterInstanceId = reader.ReadInt64();
	}
}
