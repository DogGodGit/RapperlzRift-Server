namespace ClientCommon;

public class PDRuinsReclaimRewardObjectInstance : PDPacketData
{
	public long instanceId;

	public int arrangeNo;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(arrangeNo);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		arrangeNo = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
