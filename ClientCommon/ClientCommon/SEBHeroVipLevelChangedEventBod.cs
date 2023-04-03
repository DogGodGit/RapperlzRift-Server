using System;

namespace ClientCommon;

public class SEBHeroVipLevelChangedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int vipLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(vipLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		vipLevel = reader.ReadInt32();
	}
}
