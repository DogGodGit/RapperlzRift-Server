namespace ClientCommon;

public class SEBOpen7DayEventProgressCountUpdatedEventBody : SEBServerEventBody
{
	public PDHeroOpen7DayEventProgressCount progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressCount = reader.ReadPDPacketData<PDHeroOpen7DayEventProgressCount>();
	}
}
