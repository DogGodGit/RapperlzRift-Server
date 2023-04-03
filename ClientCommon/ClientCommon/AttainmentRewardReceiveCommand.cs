namespace ClientCommon;

public class AttainmentRewardReceiveCommandBody : CommandBody
{
	public int entryNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(entryNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		entryNo = reader.ReadInt32();
	}
}
