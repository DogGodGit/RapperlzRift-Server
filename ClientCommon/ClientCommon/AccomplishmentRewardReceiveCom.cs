namespace ClientCommon;

public class AccomplishmentRewardReceiveCommandBody : CommandBody
{
	public int accomplishmentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(accomplishmentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		accomplishmentId = reader.ReadInt32();
	}
}
