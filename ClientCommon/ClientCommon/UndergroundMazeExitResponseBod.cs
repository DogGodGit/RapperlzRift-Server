using System;

namespace ClientCommon;

public class UndergroundMazeExitResponseBody : ResponseBody
{
	public int previousContinentId;

	public int previousNationId;

	public DateTime date;

	public float playTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
		writer.Write(date);
		writer.Write(playTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
		date = reader.ReadDateTime();
		playTime = reader.ReadSingle();
	}
}
