using System;

namespace ClientCommon;

public class GuildInvitationRefuseCommandBody : CommandBody
{
	public Guid invitationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(invitationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		invitationId = reader.ReadGuid();
	}
}
