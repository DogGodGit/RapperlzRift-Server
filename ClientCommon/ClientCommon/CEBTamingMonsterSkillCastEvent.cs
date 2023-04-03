using System;

namespace ClientCommon;

public class CEBTamingMonsterSkillCastEventBody : CEBClientEventBody
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
