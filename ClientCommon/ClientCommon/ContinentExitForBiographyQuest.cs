namespace ClientCommon;

public class ContinentExitForBiographyQuestDungeonEnterCommandBody : CommandBody
{
	public int dungeonId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(dungeonId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		dungeonId = reader.ReadInt32();
	}
}
public class ContinentExitForBiographyQuestDungeonEnterResponseBody : ResponseBody
{
}
