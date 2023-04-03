using System;

namespace ClientCommon;

public class DailyQuestRefreshResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyQuestFreeRefreshCount;

	public long gold;

	public PDHeroDailyQuest[] addedDailyQuests;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyQuestFreeRefreshCount);
		writer.Write(gold);
		writer.Write(addedDailyQuests);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyQuestFreeRefreshCount = reader.ReadInt32();
		gold = reader.ReadInt64();
		addedDailyQuests = reader.ReadPDPacketDatas<PDHeroDailyQuest>();
	}
}
