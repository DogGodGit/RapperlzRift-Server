using System;

namespace ClientCommon;

public class SEBPartyMemberExitEventBody : SEBServerEventBody
{
	public Guid memberId;

	public bool banished;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(memberId);
		writer.Write(banished);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		memberId = reader.ReadGuid();
		banished = reader.ReadBoolean();
	}
}
