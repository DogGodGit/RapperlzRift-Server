namespace ClientCommon;

public class PDAbnormalStateEffect : PDPacketData
{
	public long instanceId;

	public int abnormalStateId;

	public int sourceJobId;

	public int level;

	public float remainingTime;

	public int damageAbsorbShieldRemainingAbsorbAmount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(abnormalStateId);
		writer.Write(sourceJobId);
		writer.Write(level);
		writer.Write(remainingTime);
		writer.Write(damageAbsorbShieldRemainingAbsorbAmount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		abnormalStateId = reader.ReadInt32();
		sourceJobId = reader.ReadInt32();
		level = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		damageAbsorbShieldRemainingAbsorbAmount = reader.ReadInt32();
	}
}
