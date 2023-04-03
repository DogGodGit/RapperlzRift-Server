using System;

namespace ClientCommon;

public class ChattingMessageSendCommandBody : CommandBody
{
	public int type;

	public string[] messages;

	public PDChattingLink link;

	public Guid targetHeroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(type);
		writer.Write(messages);
		writer.Write(link);
		writer.Write(targetHeroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		type = reader.ReadInt32();
		messages = reader.ReadStrings();
		link = reader.ReadPDChattingLink();
		targetHeroId = reader.ReadGuid();
	}
}
