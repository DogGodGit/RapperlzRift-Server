using System;

namespace ClientCommon;

public class BlacklistEntryDeleteCommandBody : CommandBody
{
	public Guid targetHeroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetHeroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetHeroId = reader.ReadGuid();
	}
}
