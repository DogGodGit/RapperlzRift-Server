namespace ClientCommon;

public class PDProofOfValorBuffBoxInstance : PDPacketData
{
	public long instanceId;

	public int arrangeId;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(arrangeId);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		arrangeId = reader.ReadInt32();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
