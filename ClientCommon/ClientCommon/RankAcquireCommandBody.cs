namespace ClientCommon;

public class RankAcquireCommandBody : CommandBody
{
	public int targetRankNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetRankNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetRankNo = reader.ReadInt32();
	}
}
