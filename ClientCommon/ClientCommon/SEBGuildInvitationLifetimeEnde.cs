using System;

namespace ClientCommon;

public class SEBGuildInvitationLifetimeEndedEventBody : SEBServerEventBody
{
	public Guid invitationId;

	public Guid targetId;

	public string targetName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(invitationId);
		writer.Write(targetId);
		writer.Write(targetName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		invitationId = reader.ReadGuid();
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
	}
}
