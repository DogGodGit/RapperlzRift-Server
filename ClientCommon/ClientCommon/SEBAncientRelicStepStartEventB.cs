namespace ClientCommon;

public class SEBAncientRelicStepStartEventBody : SEBServerEventBody
{
	public int stepNo;

	public PDVector3 targetPosition;

	public float targetRadius;

	public int removeObstacleId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(targetPosition);
		writer.Write(targetRadius);
		writer.Write(removeObstacleId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		targetPosition = reader.ReadPDVector3();
		targetRadius = reader.ReadSingle();
		removeObstacleId = reader.ReadInt32();
	}
}
