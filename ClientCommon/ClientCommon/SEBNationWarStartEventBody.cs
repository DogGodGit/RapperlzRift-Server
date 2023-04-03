using System;

namespace ClientCommon;

public class SEBNationWarStartEventBody : SEBServerEventBody
{
	public Guid declarationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(declarationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		declarationId = reader.ReadGuid();
	}
}
