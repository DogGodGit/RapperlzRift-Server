namespace ClientCommon;

public class SubGearLevelUpTotallyResponseBody : ResponseBody
{
	public int subGearLevel;

	public int subGearQuality;

	public long gold;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearLevel);
		writer.Write(subGearQuality);
		writer.Write(gold);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearLevel = reader.ReadInt32();
		subGearQuality = reader.ReadInt32();
		gold = reader.ReadInt64();
		maxHp = reader.ReadInt32();
	}
}
