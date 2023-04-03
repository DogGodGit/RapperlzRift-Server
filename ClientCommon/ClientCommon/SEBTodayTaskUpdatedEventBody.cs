using System;

namespace ClientCommon;

public class SEBTodayTaskUpdatedEventBody : SEBServerEventBody
{
	public DateTime date;

	public int taskId;

	public int progressCount;

	public int achievementDailyPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(taskId);
		writer.Write(progressCount);
		writer.Write(achievementDailyPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		taskId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		achievementDailyPoint = reader.ReadInt32();
	}
}
