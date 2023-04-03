namespace ClientCommon;

public class TradeShipMatchingStartResponseBody : ResponseBody
{
	public int matchingStatus;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(matchingStatus);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		matchingStatus = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
