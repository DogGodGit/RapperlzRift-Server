namespace ClientCommon;

public class SEBGuildCallEventBody : SEBServerEventBody
{
	public PDGuildCall call;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(call);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		call = reader.ReadPDPacketData<PDGuildCall>();
	}
}
