using System;

namespace ClientCommon;

public class SEBTodayMissionListChangedEventBody : SEBServerEventBody
{
	public DateTime date;

	public PDHeroTodayMission[] missions;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(missions);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		missions = reader.ReadPDPacketDatas<PDHeroTodayMission>();
	}
}
