namespace ClientCommon;

public class PDAccountChargeEvent : PDPacketData
{
	public int eventId;

	public int accUnOwnDia;

	public int[] rewardedMissions;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eventId);
		writer.Write(accUnOwnDia);
		writer.Write(rewardedMissions);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eventId = reader.ReadInt32();
		accUnOwnDia = reader.ReadInt32();
		rewardedMissions = reader.ReadInts();
	}
}
