using System;

namespace ClientCommon;

public class MailReceiveCommandBody : CommandBody
{
	public Guid mailId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mailId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mailId = reader.ReadGuid();
	}
}
