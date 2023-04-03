namespace ClientCommon;

public class SEBNewMailEventBody : SEBServerEventBody
{
	public PDMail mail;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mail);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mail = reader.ReadPDPacketData<PDMail>();
	}
}
