using System;

namespace ClientCommon;

public class SEBNationWarFinishedEventBody : SEBServerEventBody
{
	public Guid declarationId;

	public int winNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(declarationId);
		writer.Write(winNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		declarationId = reader.ReadGuid();
		winNationId = reader.ReadInt32();
	}
}
