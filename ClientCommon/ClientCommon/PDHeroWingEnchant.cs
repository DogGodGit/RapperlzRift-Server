namespace ClientCommon;

public class PDHeroWingEnchant : PDPacketData
{
	public int step;

	public int level;

	public int enchantCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(step);
		writer.Write(level);
		writer.Write(enchantCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		step = reader.ReadInt32();
		level = reader.ReadInt32();
		enchantCount = reader.ReadInt32();
	}
}
