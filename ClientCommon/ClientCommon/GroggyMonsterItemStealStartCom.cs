namespace ClientCommon;

public class GroggyMonsterItemStealStartCommandBody : CommandBody
{
	public long targetMonsterInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetMonsterInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetMonsterInstanceId = reader.ReadInt64();
	}
}
