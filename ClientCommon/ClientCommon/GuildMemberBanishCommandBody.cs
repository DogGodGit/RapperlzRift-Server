using System;

namespace ClientCommon;

public class GuildMemberBanishCommandBody : CommandBody
{
	public Guid targetMemberId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetMemberId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetMemberId = reader.ReadGuid();
	}
}
