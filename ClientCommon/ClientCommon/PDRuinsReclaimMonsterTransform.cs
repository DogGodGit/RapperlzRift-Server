namespace ClientCommon;

public class PDRuinsReclaimMonsterTransformationCancelObjectInstance : PDPacketData
{
	public long instanceId;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		position = reader.ReadPDVector3();
	}
}
