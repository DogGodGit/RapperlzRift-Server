using System;

namespace ClientCommon;

public class SEBHeroPresentEventBody : SEBServerEventBody
{
	public Guid senderId;

	public string senderName;

	public int senderNationId;

	public Guid targetId;

	public string targetName;

	public int targetNationId;

	public int presentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(senderId);
		writer.Write(senderName);
		writer.Write(senderNationId);
		writer.Write(targetId);
		writer.Write(targetName);
		writer.Write(targetNationId);
		writer.Write(presentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		senderId = reader.ReadGuid();
		senderName = reader.ReadString();
		senderNationId = reader.ReadInt32();
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
		targetNationId = reader.ReadInt32();
		presentId = reader.ReadInt32();
	}
}
