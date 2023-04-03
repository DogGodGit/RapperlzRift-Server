namespace ClientCommon;

public class ContinentExitForUndergroundMazeEnterCommandBody : CommandBody
{
	public int floor;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(floor);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		floor = reader.ReadInt32();
	}
}
public class ContinentExitForUndergroundMazeEnterResponseBody : ResponseBody
{
}
