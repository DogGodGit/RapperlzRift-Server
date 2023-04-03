namespace ClientCommon;

public class SEBOrdealQuestSlotProgressCountsUpdatedEventBody : SEBServerEventBody
{
	public PDHeroOrdealQuestSlotProgressCount[] progressCounts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCounts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCounts = reader.ReadPDPacketDatas<PDHeroOrdealQuestSlotProgressCount>();
	}
}
