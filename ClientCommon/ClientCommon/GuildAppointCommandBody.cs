using System;

namespace ClientCommon;

public class GuildAppointCommandBody : CommandBody
{
	public Guid targetMemberId;

	public int targetMemberGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetMemberId);
		writer.Write(targetMemberGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetMemberId = reader.ReadGuid();
		targetMemberGrade = reader.ReadInt32();
	}
}
