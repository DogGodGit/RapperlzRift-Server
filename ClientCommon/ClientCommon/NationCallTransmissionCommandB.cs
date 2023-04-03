namespace ClientCommon;

public class NationCallTransmissionCommandBody : CommandBody
{
	public long callId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(callId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		callId = reader.ReadInt64();
	}
}
