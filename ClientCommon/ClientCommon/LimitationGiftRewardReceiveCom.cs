namespace ClientCommon;

public class LimitationGiftRewardReceiveCommandBody : CommandBody
{
	public int scheduleId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(scheduleId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		scheduleId = reader.ReadInt32();
	}
}
