namespace ClientCommon;

public class SEBWarMemoryPointAcquisitionEventBody : SEBServerEventBody
{
	public int point;

	public long pointUpdatedTimeTicks;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(point);
		writer.Write(pointUpdatedTimeTicks);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		point = reader.ReadInt32();
		pointUpdatedTimeTicks = reader.ReadInt64();
	}
}
