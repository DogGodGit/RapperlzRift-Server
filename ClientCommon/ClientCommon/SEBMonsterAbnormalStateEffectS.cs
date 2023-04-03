namespace ClientCommon;

public class SEBMonsterAbnormalStateEffectStartEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public long abnormalStateEffectInstanceId;

	public int abnormalStateId;

	public int sourceJobId;

	public int level;

	public float remainingTime;

	public int damageAbsorbShieldRemainingAbsorbAmount;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(abnormalStateEffectInstanceId);
		writer.Write(abnormalStateId);
		writer.Write(sourceJobId);
		writer.Write(level);
		writer.Write(remainingTime);
		writer.Write(damageAbsorbShieldRemainingAbsorbAmount);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		abnormalStateEffectInstanceId = reader.ReadInt64();
		abnormalStateId = reader.ReadInt32();
		sourceJobId = reader.ReadInt32();
		level = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		damageAbsorbShieldRemainingAbsorbAmount = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
