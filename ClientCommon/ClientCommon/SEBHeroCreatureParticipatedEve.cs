using System;

namespace ClientCommon;

public class SEBHeroCreatureParticipatedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int creatureId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(creatureId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		creatureId = reader.ReadInt32();
	}
}
