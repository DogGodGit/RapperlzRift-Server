namespace ClientCommon;

public class GuildTerritoryExitResponseBody : ResponseBody
{
	public int previousContinentId;

	public int previousNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
	}
}
