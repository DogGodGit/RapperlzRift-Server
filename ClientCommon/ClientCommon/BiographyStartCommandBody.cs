namespace ClientCommon;

public class BiographyStartCommandBody : CommandBody
{
	public int biographyId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(biographyId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		biographyId = reader.ReadInt32();
	}
}
