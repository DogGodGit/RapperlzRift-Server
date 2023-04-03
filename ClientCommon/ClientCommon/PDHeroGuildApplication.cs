using System;

namespace ClientCommon;

public class PDHeroGuildApplication : PDPacketData
{
	public Guid id;

	public Guid guildId;

	public string guildName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(guildId);
		writer.Write(guildName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
	}
}
