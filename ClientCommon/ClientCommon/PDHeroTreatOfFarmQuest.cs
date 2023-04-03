namespace ClientCommon;

public class PDHeroTreatOfFarmQuest : PDPacketData
{
	public int completedMissionCount;

	public bool completed;

	public PDHeroTreatOfFarmQuestMission currentMission;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(completedMissionCount);
		writer.Write(completed);
		writer.Write(currentMission);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		completedMissionCount = reader.ReadInt32();
		completed = reader.ReadBoolean();
		currentMission = reader.ReadPDPacketData<PDHeroTreatOfFarmQuestMission>();
	}
}
