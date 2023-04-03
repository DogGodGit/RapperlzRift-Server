namespace ClientCommon;

public class PDCostumeEnchantmentSystemMessage : PDSystemMessage
{
	public int costumeId;

	public int enchantLevel;

	public override int id => 6;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(costumeId);
		writer.Write(enchantLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		costumeId = reader.ReadInt32();
		enchantLevel = reader.ReadInt32();
	}
}
