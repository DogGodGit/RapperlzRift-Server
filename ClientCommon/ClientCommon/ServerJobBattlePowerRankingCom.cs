namespace ClientCommon;

public class ServerJobBattlePowerRankingCommandBody : CommandBody
{
	public int jobId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(jobId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		jobId = reader.ReadInt32();
	}
}
