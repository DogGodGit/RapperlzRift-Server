namespace ClientCommon;

public class PDHeroGuildSkill : PDPacketData
{
	public int id;

	public int level;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(level);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
		level = reader.ReadInt32();
	}
}
