namespace ClientCommon;

public class SEBCartHitEventBody : SEBServerEventBody
{
	public long cartInstanceId;

	public PDHitResult hitResult;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInstanceId);
		writer.Write(hitResult);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInstanceId = reader.ReadInt64();
		hitResult = reader.ReadPDPacketData<PDHitResult>();
	}
}
