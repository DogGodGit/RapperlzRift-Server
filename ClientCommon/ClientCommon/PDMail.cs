using System;

namespace ClientCommon;

public class PDMail : PDPacketData
{
	public Guid id;

	public int titleType;

	public string title;

	public int contentType;

	public string content;

	public float remainingTime;

	public bool received;

	public PDMailAttachment[] attachments;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(titleType);
		writer.Write(title);
		writer.Write(contentType);
		writer.Write(content);
		writer.Write(remainingTime);
		writer.Write(received);
		writer.Write(attachments);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		titleType = reader.ReadInt32();
		title = reader.ReadString();
		contentType = reader.ReadInt32();
		content = reader.ReadString();
		remainingTime = reader.ReadSingle();
		received = reader.ReadBoolean();
		attachments = reader.ReadPDPacketDatas<PDMailAttachment>();
	}
}
