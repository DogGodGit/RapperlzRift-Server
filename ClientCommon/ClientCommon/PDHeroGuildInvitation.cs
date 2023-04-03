using System;

namespace ClientCommon;

public class PDHeroGuildInvitation : PDPacketData
{
	public Guid id;

	public Guid guildId;

	public string guildName;

	public Guid inviterId;

	public string inviterName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(guildId);
		writer.Write(guildName);
		writer.Write(inviterId);
		writer.Write(inviterName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
		inviterId = reader.ReadGuid();
		inviterName = reader.ReadString();
	}
}
