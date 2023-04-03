namespace ClientCommon;

public class FriendListResponseBody : ResponseBody
{
	public PDFriend[] friends;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(friends);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		friends = reader.ReadPDPacketDatas<PDFriend>();
	}
}
