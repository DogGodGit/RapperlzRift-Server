using System;

namespace ClientCommon;

public class AchievementRewardReceiveCommandBody : CommandBody
{
	public DateTime date;

	public int rewardNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(rewardNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		rewardNo = reader.ReadInt32();
	}
}
