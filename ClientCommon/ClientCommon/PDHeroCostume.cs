namespace ClientCommon;

public class PDHeroCostume : PDPacketData
{
	public int costumeId;

	public int costumeEffectId;

	public int enchantLevel;

	public int luckyValue;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(costumeId);
		writer.Write(costumeEffectId);
		writer.Write(enchantLevel);
		writer.Write(luckyValue);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		costumeId = reader.ReadInt32();
		costumeEffectId = reader.ReadInt32();
		enchantLevel = reader.ReadInt32();
		luckyValue = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
