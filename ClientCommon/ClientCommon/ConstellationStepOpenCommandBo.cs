namespace ClientCommon;

public class ConstellationStepOpenCommandBody : CommandBody
{
	public int constellationId;

	public int step;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(constellationId);
		writer.Write(step);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		constellationId = reader.ReadInt32();
		step = reader.ReadInt32();
	}
}
