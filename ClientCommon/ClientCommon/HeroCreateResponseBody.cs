using System;

namespace ClientCommon;

public class HeroCreateResponseBody : ResponseBody
{
	public Guid heroId;

	public int level;

	public long battlePower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(level);
		writer.Write(battlePower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		level = reader.ReadInt32();
		battlePower = reader.ReadInt64();
	}
}
