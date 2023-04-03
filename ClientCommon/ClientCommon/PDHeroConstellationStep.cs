namespace ClientCommon;

public class PDHeroConstellationStep : PDPacketData
{
	public int step;

	public int cycle;

	public int entryNo;

	public int failPoint;

	public bool activated;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(step);
		writer.Write(cycle);
		writer.Write(entryNo);
		writer.Write(failPoint);
		writer.Write(activated);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		step = reader.ReadInt32();
		cycle = reader.ReadInt32();
		entryNo = reader.ReadInt32();
		failPoint = reader.ReadInt32();
		activated = reader.ReadBoolean();
	}
}
