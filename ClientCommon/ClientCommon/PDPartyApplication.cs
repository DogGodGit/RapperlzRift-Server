using System;

namespace ClientCommon;

public class PDPartyApplication : PDPacketData
{
	public long no;

	public Guid partyId;

	public Guid applicantId;

	public string applicantName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
		writer.Write(partyId);
		writer.Write(applicantId);
		writer.Write(applicantName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt64();
		partyId = reader.ReadGuid();
		applicantId = reader.ReadGuid();
		applicantName = reader.ReadString();
	}
}
