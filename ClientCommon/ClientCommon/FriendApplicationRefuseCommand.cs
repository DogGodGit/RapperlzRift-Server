namespace ClientCommon;

public class FriendApplicationRefuseCommandBody : CommandBody
{
	public long applicationNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applicationNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applicationNo = reader.ReadInt64();
	}
}
