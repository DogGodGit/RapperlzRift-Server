using System;

namespace ClientCommon;

public class SEBHeroMaxHpChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
