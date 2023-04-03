namespace ClientCommon;

public class PDGuildBuildingInstance : PDPacketData
{
	public int buildingId;

	public int level;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buildingId);
		writer.Write(level);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buildingId = reader.ReadInt32();
		level = reader.ReadInt32();
	}
}
