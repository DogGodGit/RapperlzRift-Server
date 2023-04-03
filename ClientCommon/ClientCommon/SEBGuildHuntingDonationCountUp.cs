namespace ClientCommon;

public class SEBGuildHuntingDonationCountUpdatedEventBody : SEBServerEventBody
{
	public int donationCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(donationCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		donationCount = reader.ReadInt32();
	}
}
