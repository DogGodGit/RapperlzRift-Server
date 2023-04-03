using System;

namespace ClientCommon;

public class SEBHeroDisplayTitleChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int titleId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(titleId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		titleId = reader.ReadInt32();
	}
}
