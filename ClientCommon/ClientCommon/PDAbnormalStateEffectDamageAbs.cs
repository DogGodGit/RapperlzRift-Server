namespace ClientCommon;

public class PDAbnormalStateEffectDamageAbsorbShield : PDPacketData
{
	public long abnormalStateEffectInstanceId;

	public int remainingAbsorbShieldAmount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(abnormalStateEffectInstanceId);
		writer.Write(remainingAbsorbShieldAmount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		abnormalStateEffectInstanceId = reader.ReadInt64();
		remainingAbsorbShieldAmount = reader.ReadInt32();
	}
}
