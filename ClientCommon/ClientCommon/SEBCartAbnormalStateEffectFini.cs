namespace ClientCommon;

public class SEBCartAbnormalStateEffectFinishedEventBody : SEBServerEventBody
{
	public long cartInstanceId;

	public long abnormalStateEffectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInstanceId);
		writer.Write(abnormalStateEffectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInstanceId = reader.ReadInt64();
		abnormalStateEffectInstanceId = reader.ReadInt64();
	}
}
