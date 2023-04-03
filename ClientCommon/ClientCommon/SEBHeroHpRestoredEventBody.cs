using System;

namespace ClientCommon;

public class SEBHeroHpRestoredEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		hp = reader.ReadInt32();
	}
}
