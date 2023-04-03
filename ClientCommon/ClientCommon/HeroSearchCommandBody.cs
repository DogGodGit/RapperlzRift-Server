namespace ClientCommon;

public class HeroSearchCommandBody : CommandBody
{
	public string searchName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(searchName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		searchName = reader.ReadString();
	}
}
