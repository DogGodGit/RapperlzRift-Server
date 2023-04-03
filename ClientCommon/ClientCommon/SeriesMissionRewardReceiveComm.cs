namespace ClientCommon;

public class SeriesMissionRewardReceiveCommandBody : CommandBody
{
	public int missionId;

	public int step;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(missionId);
		writer.Write(step);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		missionId = reader.ReadInt32();
		step = reader.ReadInt32();
	}
}
