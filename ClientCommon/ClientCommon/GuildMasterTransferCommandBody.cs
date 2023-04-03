using System;

namespace ClientCommon;

public class GuildMasterTransferCommandBody : CommandBody
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
