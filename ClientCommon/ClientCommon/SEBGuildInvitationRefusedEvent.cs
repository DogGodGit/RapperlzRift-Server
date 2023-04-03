using System;

namespace ClientCommon;

public class SEBGuildInvitationRefusedEventBody : SEBServerEventBody
{
	public Guid targetId;

	public string targetName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetId);
		writer.Write(targetName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
	}
}
