namespace ClientCommon;

public class PDHeroTodayMission : PDPacketData
{
	public int missionId;

	public int progressCount;

	public bool rewardReceived;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(progressCount);
		writer.Write(rewardReceived);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		rewardReceived = reader.ReadBoolean();
	}
}
