using System;

namespace ClientCommon;

public class ArtifactRoomInitResponseBody : ResponseBody
{
	public DateTime date;

	public int currentFloor;

	public int dailyInitCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(currentFloor);
		writer.Write(dailyInitCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		currentFloor = reader.ReadInt32();
		dailyInitCount = reader.ReadInt32();
	}
}
