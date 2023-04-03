namespace ClientCommon;

public class ContinentExitForOsirisRoomEnterCommandBody : CommandBody
{
	public int difficulty;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(difficulty);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		difficulty = reader.ReadInt32();
	}
}
public class ContinentExitForOsirisRoomEnterResponseBody : ResponseBody
{
}
