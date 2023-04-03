namespace ClientCommon;

public class PDHeroDimensionRaidQuest : PDPacketData
{
	public int step;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(step);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		step = reader.ReadInt32();
	}
}
