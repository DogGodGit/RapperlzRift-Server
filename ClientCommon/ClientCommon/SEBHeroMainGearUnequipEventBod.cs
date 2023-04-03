using System;

namespace ClientCommon;

public class SEBHeroMainGearUnequipEventBody : SEBServerEventBody
{
	public Guid heroId;

	public Guid heroMainGearId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(heroMainGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		heroMainGearId = reader.ReadGuid();
	}
}
