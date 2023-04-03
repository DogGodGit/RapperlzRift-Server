using System;

namespace ClientCommon;

public class SEBHeroInfiniteWarBuffBoxAcquisitionEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int hp;

	public long instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(hp);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		hp = reader.ReadInt32();
		instanceId = reader.ReadInt64();
	}
}
