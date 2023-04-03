using System;

namespace ClientCommon;

public class GetTimeResponseBody : ResponseBody
{
	public DateTimeOffset time;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(time);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		time = reader.ReadDateTimeOffset();
	}
}
