using System;

namespace ClientCommon;

public class SEBPresentReceivedEventBody : SEBServerEventBody
{
	public Guid senderId;

	public string senderName;

	public int senderNationId;

	public int presentId;

	public DateTime weekStartDate;

	public int weeklyPresentPopularityPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(senderId);
		writer.Write(senderName);
		writer.Write(senderNationId);
		writer.Write(presentId);
		writer.Write(weekStartDate);
		writer.Write(weeklyPresentPopularityPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		senderId = reader.ReadGuid();
		senderName = reader.ReadString();
		senderNationId = reader.ReadInt32();
		presentId = reader.ReadInt32();
		weekStartDate = reader.ReadDateTime();
		weeklyPresentPopularityPoint = reader.ReadInt32();
	}
}
