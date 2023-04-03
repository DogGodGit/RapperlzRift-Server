using System;

namespace ClientCommon;

public class GuildApplyCommandBody : CommandBody
{
	public Guid guildId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guildId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guildId = reader.ReadGuid();
	}
}
