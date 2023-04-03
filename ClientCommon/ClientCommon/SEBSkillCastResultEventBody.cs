using System;

namespace ClientCommon;

public class SEBSkillCastResultEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int skillId;

	public int chainSkillId;

	public bool isSucceeded;

	public int lak;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(skillId);
		writer.Write(chainSkillId);
		writer.Write(isSucceeded);
		writer.Write(lak);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		chainSkillId = reader.ReadInt32();
		isSucceeded = reader.ReadBoolean();
		lak = reader.ReadInt32();
	}
}
