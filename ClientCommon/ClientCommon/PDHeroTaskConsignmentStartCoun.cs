namespace ClientCommon;

public class PDHeroTaskConsignmentStartCount : PDPacketData
{
	public int consignmentId;

	public int startCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(consignmentId);
		writer.Write(startCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		consignmentId = reader.ReadInt32();
		startCount = reader.ReadInt32();
	}
}
