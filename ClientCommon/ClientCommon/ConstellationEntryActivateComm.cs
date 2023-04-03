namespace ClientCommon;

public class ConstellationEntryActivateCommandBody : CommandBody
{
	public int constellationId;

	public int step;

	public int cycle;

	public int entryNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(constellationId);
		writer.Write(step);
		writer.Write(cycle);
		writer.Write(entryNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		constellationId = reader.ReadInt32();
		step = reader.ReadInt32();
		cycle = reader.ReadInt32();
		entryNo = reader.ReadInt32();
	}
}
