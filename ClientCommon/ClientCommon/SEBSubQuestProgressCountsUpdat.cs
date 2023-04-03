namespace ClientCommon;

public class SEBSubQuestProgressCountsUpdatedEventBody : SEBServerEventBody
{
	public PDHeroSubQuestProgressCount[] progressCounts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCounts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCounts = reader.ReadPDPacketDatas<PDHeroSubQuestProgressCount>();
	}
}
