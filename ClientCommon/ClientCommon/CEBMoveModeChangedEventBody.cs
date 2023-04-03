namespace ClientCommon;

public class CEBMoveModeChangedEventBody : CEBClientEventBody
{
	public bool isWalking;

	public bool isManualMoving;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isWalking);
		writer.Write(isManualMoving);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isWalking = reader.ReadBoolean();
		isManualMoving = reader.ReadBoolean();
	}
}
