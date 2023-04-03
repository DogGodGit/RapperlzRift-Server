namespace ClientCommon;

public class SEBTodayMissionUpdatedEventBody : SEBServerEventBody
{
	public int missionId;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
