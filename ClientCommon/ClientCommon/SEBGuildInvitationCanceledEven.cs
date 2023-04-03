using System;

namespace ClientCommon;

public class SEBGuildInvitationCanceledEventBody : SEBServerEventBody
{
	public Guid id;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
	}
}
