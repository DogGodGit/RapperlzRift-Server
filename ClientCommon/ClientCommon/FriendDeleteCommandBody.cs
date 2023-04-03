using System;

namespace ClientCommon;

public class FriendDeleteCommandBody : CommandBody
{
	public Guid[] friendIds;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(friendIds);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		friendIds = reader.ReadGuids();
	}
}
