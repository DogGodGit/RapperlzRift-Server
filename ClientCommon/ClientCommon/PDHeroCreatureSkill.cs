namespace ClientCommon;

public class PDHeroCreatureSkill : PDPacketData
{
	public int slotIndex;

	public int skillId;

	public int grade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(slotIndex);
		writer.Write(skillId);
		writer.Write(grade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		slotIndex = reader.ReadInt32();
		skillId = reader.ReadInt32();
		grade = reader.ReadInt32();
	}
}
