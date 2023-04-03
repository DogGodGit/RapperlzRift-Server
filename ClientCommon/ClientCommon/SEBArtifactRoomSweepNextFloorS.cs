namespace ClientCommon;

public class SEBArtifactRoomSweepNextFloorStartEventBody : SEBServerEventBody
{
	public int progressFloor;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(progressFloor);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		progressFloor = reader.ReadInt32();
	}
}
