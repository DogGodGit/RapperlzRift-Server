using System;

namespace ClientCommon;

public class PDHeroWeeklyQuest : PDPacketData
{
	public DateTime weekStartDate;

	public int roundNo;

	public Guid roundId;

	public int roundMissionId;

	public bool isRoundAccepted;

	public int roundProgressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(weekStartDate);
		writer.Write(roundNo);
		writer.Write(roundId);
		writer.Write(roundMissionId);
		writer.Write(isRoundAccepted);
		writer.Write(roundProgressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		weekStartDate = reader.ReadDateTime();
		roundNo = reader.ReadInt32();
		roundId = reader.ReadGuid();
		roundMissionId = reader.ReadInt32();
		isRoundAccepted = reader.ReadBoolean();
		roundProgressCount = reader.ReadInt32();
	}
}
