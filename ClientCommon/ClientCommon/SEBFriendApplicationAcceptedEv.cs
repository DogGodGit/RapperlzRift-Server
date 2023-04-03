namespace ClientCommon;

public class SEBFriendApplicationAcceptedEventBody : SEBServerEventBody
{
	public long applicationNo;

	public PDFriend newFriend;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(applicationNo);
		writer.Write(newFriend);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		applicationNo = reader.ReadInt64();
		newFriend = reader.ReadPDPacketData<PDFriend>();
	}
}
