namespace ClientCommon;

public class CreatureCardDisassembleAllCommandBody : CommandBody
{
	public int categoryId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(categoryId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		categoryId = reader.ReadInt32();
	}
}
