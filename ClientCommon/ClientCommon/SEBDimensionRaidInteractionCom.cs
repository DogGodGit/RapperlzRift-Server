namespace ClientCommon;

public class SEBDimensionRaidInteractionCompletedEventBody : SEBServerEventBody
{
	public int nextStep;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nextStep);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nextStep = reader.ReadInt32();
	}
}
