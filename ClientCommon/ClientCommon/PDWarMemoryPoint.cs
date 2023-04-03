using System;

namespace ClientCommon;

public class PDWarMemoryPoint : PDPacketData
{
	public Guid heroId;

	public string name;

	public int point;

	public long pointUpdatedTimeTicks;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(point);
		writer.Write(pointUpdatedTimeTicks);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		point = reader.ReadInt32();
		pointUpdatedTimeTicks = reader.ReadInt64();
	}
}
