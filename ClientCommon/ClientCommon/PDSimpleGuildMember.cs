using System;

namespace ClientCommon;

public class PDSimpleGuildMember : PDPacketData
{
	public Guid memberId;

	public string memberName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(memberId);
		writer.Write(memberName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		memberId = reader.ReadGuid();
		memberName = reader.ReadString();
	}
}
