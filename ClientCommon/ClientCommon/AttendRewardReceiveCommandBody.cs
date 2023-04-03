namespace ClientCommon;

public class AttendRewardReceiveCommandBody : CommandBody
{
	public int attendDay;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(attendDay);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		attendDay = reader.ReadInt32();
	}
}
