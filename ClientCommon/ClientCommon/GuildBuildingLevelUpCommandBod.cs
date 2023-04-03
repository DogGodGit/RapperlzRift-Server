namespace ClientCommon;

public class GuildBuildingLevelUpCommandBody : CommandBody
{
	public int buildingId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buildingId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buildingId = reader.ReadInt32();
	}
}
