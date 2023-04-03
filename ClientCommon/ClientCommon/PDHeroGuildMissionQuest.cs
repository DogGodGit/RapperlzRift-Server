using System;

namespace ClientCommon;

public class PDHeroGuildMissionQuest : PDPacketData
{
	public DateTime date;

	public int completedMissionCount;

	public bool completed;

	public PDHeroGuildMission currentMission;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(completedMissionCount);
		writer.Write(completed);
		writer.Write(currentMission);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		completedMissionCount = reader.ReadInt32();
		completed = reader.ReadBoolean();
		currentMission = reader.ReadPDPacketData<PDHeroGuildMission>();
	}
}
