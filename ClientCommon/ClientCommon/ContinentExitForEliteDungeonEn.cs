namespace ClientCommon;

public class ContinentExitForEliteDungeonEnterCommandBody : CommandBody
{
	public int eliteMonsterMasterId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eliteMonsterMasterId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eliteMonsterMasterId = reader.ReadInt32();
	}
}
public class ContinentExitForEliteDungeonEnterResponseBody : ResponseBody
{
}
