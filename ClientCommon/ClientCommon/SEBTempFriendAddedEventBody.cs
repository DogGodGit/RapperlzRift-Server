using System;

namespace ClientCommon;

public class SEBTempFriendAddedEventBody : SEBServerEventBody
{
	public PDTempFriend tempFriend;

	public Guid removedTempFriendId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(tempFriend);
		writer.Write(removedTempFriendId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		tempFriend = reader.ReadPDPacketData<PDTempFriend>();
		removedTempFriendId = reader.ReadGuid();
	}
}
