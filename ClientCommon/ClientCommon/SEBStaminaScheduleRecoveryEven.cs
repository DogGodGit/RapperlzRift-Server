namespace ClientCommon;

public class SEBStaminaScheduleRecoveryEventBody : SEBServerEventBody
{
	public int stamina;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stamina);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stamina = reader.ReadInt32();
	}
}
