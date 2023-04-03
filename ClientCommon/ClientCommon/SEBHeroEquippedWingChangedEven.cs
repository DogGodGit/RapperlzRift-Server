using System;

namespace ClientCommon;

public class SEBHeroEquippedWingChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int wingId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(wingId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		wingId = reader.ReadInt32();
	}
}
