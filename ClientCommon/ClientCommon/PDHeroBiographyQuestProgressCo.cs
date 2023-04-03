namespace ClientCommon;

public class PDHeroBiographyQuestProgressCount : PDPacketData
{
	public int biographyId;

	public int questNo;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(biographyId);
		writer.Write(questNo);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		biographyId = reader.ReadInt32();
		questNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
