using System;

namespace ClientCommon;

public class SEBPresentReplyReceivedEventBody : SEBServerEventBody
{
	public Guid senderId;

	public string senderName;

	public int senderNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(senderId);
		writer.Write(senderName);
		writer.Write(senderNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		senderId = reader.ReadGuid();
		senderName = reader.ReadString();
		senderNationId = reader.ReadInt32();
	}
}
