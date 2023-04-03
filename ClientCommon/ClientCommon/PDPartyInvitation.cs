using System;

namespace ClientCommon;

public class PDPartyInvitation : PDPacketData
{
	public long no;

	public Guid partyId;

	public Guid targetId;

	public string targetName;

	public Guid inviterId;

	public string inviterName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
		writer.Write(partyId);
		writer.Write(targetId);
		writer.Write(targetName);
		writer.Write(inviterId);
		writer.Write(inviterName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt64();
		partyId = reader.ReadGuid();
		targetId = reader.ReadGuid();
		targetName = reader.ReadString();
		inviterId = reader.ReadGuid();
		inviterName = reader.ReadString();
	}
}
