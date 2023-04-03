using System;

namespace ClientCommon;

public class SEBGuildApplicationAcceptedEventBody : SEBServerEventBody
{
	public Guid applicationId;

	public PDGuild guild;

	public int memberGrade;

	public int maxHp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applicationId);
		writer.Write(guild);
		writer.Write(memberGrade);
		writer.Write(maxHp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applicationId = reader.ReadGuid();
		guild = reader.ReadPDPacketData<PDGuild>();
		memberGrade = reader.ReadInt32();
		maxHp = reader.ReadInt32();
	}
}
