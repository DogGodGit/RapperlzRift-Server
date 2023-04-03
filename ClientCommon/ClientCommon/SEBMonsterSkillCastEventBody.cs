namespace ClientCommon;

public class SEBMonsterSkillCastEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public int skillId;

	public PDVector3 skillTargetPosition;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(skillId);
		writer.Write(skillTargetPosition);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		skillId = reader.ReadInt32();
		skillTargetPosition = reader.ReadPDVector3();
	}
}
