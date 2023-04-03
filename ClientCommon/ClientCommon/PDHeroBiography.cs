namespace ClientCommon;

public class PDHeroBiography : PDPacketData
{
	public int biographyId;

	public bool completed;

	public PDHeroBiograhyQuest quest;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(biographyId);
		writer.Write(completed);
		writer.Write(quest);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		biographyId = reader.ReadInt32();
		completed = reader.ReadBoolean();
		quest = reader.ReadPDPacketData<PDHeroBiograhyQuest>();
	}
}
