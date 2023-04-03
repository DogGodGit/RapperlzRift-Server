namespace ClientCommon;

public class SEBPartyApplicationArrivedEventBody : SEBServerEventBody
{
	public PDPartyApplication app;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(app);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		app = reader.ReadPDPacketData<PDPartyApplication>();
	}
}
