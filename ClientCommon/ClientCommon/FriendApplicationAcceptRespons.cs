namespace ClientCommon;

public class FriendApplicationAcceptResponseBody : ResponseBody
{
	public PDFriend newFriend;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(newFriend);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		newFriend = reader.ReadPDPacketData<PDFriend>();
	}
}
