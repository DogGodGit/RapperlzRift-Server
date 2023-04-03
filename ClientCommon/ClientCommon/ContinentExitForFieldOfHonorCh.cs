namespace ClientCommon;

public class ContinentExitForFieldOfHonorChallengeCommandBody : CommandBody
{
	public int targetRanking;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetRanking);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetRanking = reader.ReadInt32();
	}
}
public class ContinentExitForFieldOfHonorChallengeResponseBody : ResponseBody
{
}
