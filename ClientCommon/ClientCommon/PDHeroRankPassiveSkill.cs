namespace ClientCommon;

public class PDHeroRankPassiveSkill : PDPacketData
{
	public int skillId;

	public int level;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(skillId);
		writer.Write(level);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		skillId = reader.ReadInt32();
		level = reader.ReadInt32();
	}
}
