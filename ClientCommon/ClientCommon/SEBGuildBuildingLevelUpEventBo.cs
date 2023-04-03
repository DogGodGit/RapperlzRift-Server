namespace ClientCommon;

public class SEBGuildBuildingLevelUpEventBody : SEBServerEventBody
{
	public int buildingId;

	public int buildingLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buildingId);
		writer.Write(buildingLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buildingId = reader.ReadInt32();
		buildingLevel = reader.ReadInt32();
	}
}
