using System;

namespace ClientCommon;

public class SEBHeroCostumeEquippedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int costumeId;

	public int costumeEffectId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(costumeId);
		writer.Write(costumeEffectId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		costumeId = reader.ReadInt32();
		costumeEffectId = reader.ReadInt32();
	}
}
