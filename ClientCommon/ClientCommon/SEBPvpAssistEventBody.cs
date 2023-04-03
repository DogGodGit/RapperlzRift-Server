using System;

namespace ClientCommon;

public class SEBPvpAssistEventBody : SEBServerEventBody
{
	public DateTime date;

	public int assistCount;

	public int exploitPoint;

	public int dailyExploitPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(assistCount);
		writer.Write(exploitPoint);
		writer.Write(dailyExploitPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		assistCount = reader.ReadInt32();
		exploitPoint = reader.ReadInt32();
		dailyExploitPoint = reader.ReadInt32();
	}
}
