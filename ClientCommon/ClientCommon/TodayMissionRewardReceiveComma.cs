using System;

namespace ClientCommon;

public class TodayMissionRewardReceiveCommandBody : CommandBody
{
	public DateTime date;

	public int missionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(missionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		missionId = reader.ReadInt32();
	}
}
