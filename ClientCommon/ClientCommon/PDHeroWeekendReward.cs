using System;

namespace ClientCommon;

public class PDHeroWeekendReward : PDPacketData
{
	public DateTime weekStartDate;

	public int selection1;

	public int selection2;

	public int selection3;

	public bool rewarded;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(weekStartDate);
		writer.Write(selection1);
		writer.Write(selection2);
		writer.Write(selection3);
		writer.Write(rewarded);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		weekStartDate = reader.ReadDateTime();
		selection1 = reader.ReadInt32();
		selection2 = reader.ReadInt32();
		selection3 = reader.ReadInt32();
		rewarded = reader.ReadBoolean();
	}
}
