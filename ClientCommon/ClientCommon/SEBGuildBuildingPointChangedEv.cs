namespace ClientCommon;

public class SEBGuildBuildingPointChangedEventBody : SEBServerEventBody
{
	public int buildingPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buildingPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buildingPoint = reader.ReadInt32();
	}
}
