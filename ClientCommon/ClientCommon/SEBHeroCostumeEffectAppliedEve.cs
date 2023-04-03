using System;

namespace ClientCommon;

public class SEBHeroCostumeEffectAppliedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int costumeEffectId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(costumeEffectId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		costumeEffectId = reader.ReadInt32();
	}
}
