namespace ClientCommon;

public class ContinentEnterForReturnScrollUseCommandBody : CommandBody
{
}
public class ContinentEnterForReturnScrollUseResponseBody : ResponseBody
{
	public PDContinentEntranceInfo entranceInfo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(entranceInfo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		entranceInfo = reader.ReadPDPacketData<PDContinentEntranceInfo>();
	}
}
