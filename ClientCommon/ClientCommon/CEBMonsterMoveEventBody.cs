namespace ClientCommon;

public class CEBMonsterMoveEventBody : CEBClientEventBody
{
	public long instanceId;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
