namespace ClientCommon;

public class PDMainGearEnchantmentSystemMessage : PDSystemMessage
{
	public int mainGearId;

	public int enchantLevel;

	public override int id => 2;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainGearId);
		writer.Write(enchantLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainGearId = reader.ReadInt32();
		enchantLevel = reader.ReadInt32();
	}
}
