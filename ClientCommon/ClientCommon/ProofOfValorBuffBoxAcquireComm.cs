namespace ClientCommon;

public class ProofOfValorBuffBoxAcquireCommandBody : CommandBody
{
	public long buffBoxInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(buffBoxInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		buffBoxInstanceId = reader.ReadInt64();
	}
}
