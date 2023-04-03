using System;

namespace ClientCommon;

public class SEBHeroMountLevelUpEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int mountId;

	public int mountLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(mountId);
		writer.Write(mountLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		mountId = reader.ReadInt32();
		mountLevel = reader.ReadInt32();
	}
}
