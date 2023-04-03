namespace ClientCommon;

public class BiographyQuestAcceptCommandBody : CommandBody
{
	public int biographyId;

	public int questNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(biographyId);
		writer.Write(questNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		biographyId = reader.ReadInt32();
		questNo = reader.ReadInt32();
	}
}
