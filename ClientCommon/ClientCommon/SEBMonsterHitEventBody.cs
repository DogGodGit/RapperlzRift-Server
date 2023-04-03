namespace ClientCommon;

public class SEBMonsterHitEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public PDHitResult hitResult;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(hitResult);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		hitResult = reader.ReadPDPacketData<PDHitResult>();
	}
}
