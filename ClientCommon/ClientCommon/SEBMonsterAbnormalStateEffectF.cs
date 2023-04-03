namespace ClientCommon;

public class SEBMonsterAbnormalStateEffectFinishedEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public long abnormalStateEffectInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(abnormalStateEffectInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		abnormalStateEffectInstanceId = reader.ReadInt64();
	}
}
