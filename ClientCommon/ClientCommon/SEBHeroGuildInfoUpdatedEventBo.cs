using System;

namespace ClientCommon;

public class SEBHeroGuildInfoUpdatedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public Guid guildId;

	public string guildName;

	public int guildMemberGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(guildId);
		writer.Write(guildName);
		writer.Write(guildMemberGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
		guildMemberGrade = reader.ReadInt32();
	}
}
