namespace ClientCommon;

public class SEBWarMemoryWaveCompletedEventBody : SEBServerEventBody
{
	public PDWarMemoryPoint[] points;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(points);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		points = reader.ReadPDPacketDatas<PDWarMemoryPoint>();
	}
}
