using System;

namespace ClientCommon;

public class SEBHeroLevelUpEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int level;

	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(level);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		level = reader.ReadInt32();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
