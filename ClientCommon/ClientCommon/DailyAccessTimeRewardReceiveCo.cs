namespace ClientCommon;

public class DailyAccessTimeRewardReceiveCommandBody : CommandBody
{
	public int entryId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(entryId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		entryId = reader.ReadInt32();
	}
}
