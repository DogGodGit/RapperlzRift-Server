namespace ClientCommon;

public class PDHeroRetrieval : PDPacketData
{
	public int retrievalId;

	public int count;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(retrievalId);
		writer.Write(count);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		retrievalId = reader.ReadInt32();
		count = reader.ReadInt32();
	}
}
