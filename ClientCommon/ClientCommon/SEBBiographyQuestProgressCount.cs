namespace ClientCommon;

public class SEBBiographyQuestProgressCountsUpdatedEventBody : SEBServerEventBody
{
	public PDHeroBiographyQuestProgressCount[] progressCounts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCounts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCounts = reader.ReadPDPacketDatas<PDHeroBiographyQuestProgressCount>();
	}
}
