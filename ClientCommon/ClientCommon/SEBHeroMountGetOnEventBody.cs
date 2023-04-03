using System;

namespace ClientCommon;

public class SEBHeroMountGetOnEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int mountId;

	public int level;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(mountId);
		writer.Write(level);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		mountId = reader.ReadInt32();
		level = reader.ReadInt32();
	}
}
