using System;

namespace ClientCommon;

public class SEBGuildMemberExitEventBody : SEBServerEventBody
{
	public Guid heroId;

	public string name;

	public bool isBanished;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(isBanished);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		isBanished = reader.ReadBoolean();
	}
}
