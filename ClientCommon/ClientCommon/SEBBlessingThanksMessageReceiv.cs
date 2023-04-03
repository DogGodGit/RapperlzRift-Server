using System;

namespace ClientCommon;

public class SEBBlessingThanksMessageReceivedEventBody : SEBServerEventBody
{
	public Guid senderId;

	public string senderName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(senderId);
		writer.Write(senderName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		senderId = reader.ReadGuid();
		senderName = reader.ReadString();
	}
}
