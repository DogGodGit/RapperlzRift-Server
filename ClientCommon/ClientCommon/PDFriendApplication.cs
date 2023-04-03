using System;

namespace ClientCommon;

public class PDFriendApplication : PDPacketData
{
	public long no;

	public Guid applicantId;

	public string applicantName;

	public int applicantNationId;

	public Guid targetId;

	public string targetName;

	public int targetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
		writer.Write(applicantId);
		writer.Write(applicantName);
		writer.Write(applicantNationId);
		writer.Write(targetId);
		writer.Write(targetName);
		writer.Write(targetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt64();
		applicantId = reader.ReadGuid();
		applicantName = reader.ReadString();
		applicantNationId = reader.ReadInt32();
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
		targetNationId = reader.ReadInt32();
	}
}
