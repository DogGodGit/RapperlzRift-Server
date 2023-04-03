namespace ClientCommon;

public class SEBSoulCoveterMatchingRoomPartyEnterEventBody : SEBServerEventBody
{
	public int difficulty;

	public int matchingStatus;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(difficulty);
		writer.Write(matchingStatus);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		difficulty = reader.ReadInt32();
		matchingStatus = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
