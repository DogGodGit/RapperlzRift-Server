namespace ClientCommon;

public class RetrieveGoldCommandBody : CommandBody
{
	public int retrievalId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(retrievalId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		retrievalId = reader.ReadInt32();
	}
}
