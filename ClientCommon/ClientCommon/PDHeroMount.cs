namespace ClientCommon;

public class PDHeroMount : PDPacketData
{
	public int mountId;

	public int level;

	public int satiety;

	public int awakeningLevel;

	public int awakeningExp;

	public int potionAttrCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mountId);
		writer.Write(level);
		writer.Write(satiety);
		writer.Write(awakeningLevel);
		writer.Write(awakeningExp);
		writer.Write(potionAttrCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mountId = reader.ReadInt32();
		level = reader.ReadInt32();
		satiety = reader.ReadInt32();
		awakeningLevel = reader.ReadInt32();
		awakeningExp = reader.ReadInt32();
		potionAttrCount = reader.ReadInt32();
	}
}
