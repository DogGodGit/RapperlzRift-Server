namespace ClientCommon;

public class UndergroundMazeTransmissionCommandBody : CommandBody
{
	public int npcId;

	public int floor;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(npcId);
		writer.Write(floor);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		npcId = reader.ReadInt32();
		floor = reader.ReadInt32();
	}
}
