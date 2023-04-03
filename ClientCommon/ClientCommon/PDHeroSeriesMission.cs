namespace ClientCommon;

public class PDHeroSeriesMission : PDPacketData
{
	public int missionId;

	public int currentStep;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(currentStep);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		currentStep = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
