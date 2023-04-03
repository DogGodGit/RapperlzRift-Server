using System;

namespace ClientCommon;

public class SEBHeroWarMemoryPointAcquisitionEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int point;

	public long pointUpdatedTimeTicks;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(point);
		writer.Write(pointUpdatedTimeTicks);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		point = reader.ReadInt32();
		pointUpdatedTimeTicks = reader.ReadInt64();
	}
}
