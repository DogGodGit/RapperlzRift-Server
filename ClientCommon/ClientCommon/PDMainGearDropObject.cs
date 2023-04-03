namespace ClientCommon;

public class PDMainGearDropObject : PDDropObject
{
	public int id;

	public bool owned;

	public int enchantLevel;

	public override int type => 1;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(owned);
		writer.Write(enchantLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
		owned = reader.ReadBoolean();
		enchantLevel = reader.ReadInt32();
	}
}
