namespace ClientCommon;

public class PDAccountConsumeEvent : PDPacketData
{
	public int eventId;

	public int accDia;

	public int[] rewardedMissions;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eventId);
		writer.Write(accDia);
		writer.Write(rewardedMissions);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eventId = reader.ReadInt32();
		accDia = reader.ReadInt32();
		rewardedMissions = reader.ReadInts();
	}
}
