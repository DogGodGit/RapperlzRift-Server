using System;

namespace ClientCommon;

public class SEBGuildSpiritAnnouncedEventBody : SEBServerEventBody
{
	public Guid guildId;

	public string guildName;

	public Guid heroId;

	public string heroName;

	public int continentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(guildId);
		writer.Write(guildName);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(continentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		continentId = reader.ReadInt32();
	}
}
