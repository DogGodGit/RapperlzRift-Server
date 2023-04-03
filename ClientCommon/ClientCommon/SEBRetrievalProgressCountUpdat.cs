namespace ClientCommon;

public class SEBRetrievalProgressCountUpdatedEventBody : SEBServerEventBody
{
	public PDHeroRetrievalProgressCount progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCount = reader.ReadPDPacketData<PDHeroRetrievalProgressCount>();
	}
}
