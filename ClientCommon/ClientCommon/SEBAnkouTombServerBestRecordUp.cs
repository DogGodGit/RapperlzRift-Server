namespace ClientCommon;

public class SEBAnkouTombServerBestRecordUpdatedEventBody : SEBServerEventBody
{
	public PDHeroAnkouTombBestRecord record;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(record);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		record = reader.ReadPDPacketData<PDHeroAnkouTombBestRecord>();
	}
}
