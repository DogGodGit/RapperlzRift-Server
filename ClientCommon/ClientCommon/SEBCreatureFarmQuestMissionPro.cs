namespace ClientCommon;

public class SEBCreatureFarmQuestMissionProgressCountUpdatedEventBody : SEBServerEventBody
{
	public int missionNo;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionNo);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
