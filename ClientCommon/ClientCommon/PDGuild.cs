using System;

namespace ClientCommon;

public class PDGuild : PDPacketData
{
	public Guid id;

	public string name;

	public string notice;

	public int buildingPoint;

	public long fund;

	public int applicationCount;

	public PDGuildBuildingInstance[] buildingInsts;

	public Guid foodWarehouseCollectionId;

	public DateTime moralPointDate;

	public int moralPoint;

	public DateTime dailyGuildSupplySupportQuestStartDate;

	public int dailyGuildSupplySupportQuestStartCount;

	public DateTime dailyHuntingDonationDate;

	public int dailyHuntingDonationCount;

	public DateTime dailyObjectiveDate;

	public int dailyObjectiveContentId;

	public int dailyObjectiveCompletionMemberCount;

	public DateTime weeklyObjectiveDate;

	public int weeklyObjectiveId;

	public int weeklyObjectiveCompletionMemberCount;

	public DateTime lastBlessingBuffStartDate;

	public bool isBlessingBuffRunning;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(notice);
		writer.Write(buildingPoint);
		writer.Write(fund);
		writer.Write(applicationCount);
		writer.Write(buildingInsts);
		writer.Write(foodWarehouseCollectionId);
		writer.Write(moralPointDate);
		writer.Write(moralPoint);
		writer.Write(dailyGuildSupplySupportQuestStartDate);
		writer.Write(dailyGuildSupplySupportQuestStartCount);
		writer.Write(dailyHuntingDonationDate);
		writer.Write(dailyHuntingDonationCount);
		writer.Write(dailyObjectiveDate);
		writer.Write(dailyObjectiveContentId);
		writer.Write(dailyObjectiveCompletionMemberCount);
		writer.Write(weeklyObjectiveDate);
		writer.Write(weeklyObjectiveId);
		writer.Write(weeklyObjectiveCompletionMemberCount);
		writer.Write(lastBlessingBuffStartDate);
		writer.Write(isBlessingBuffRunning);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		notice = reader.ReadString();
		buildingPoint = reader.ReadInt32();
		fund = reader.ReadInt64();
		applicationCount = reader.ReadInt32();
		buildingInsts = reader.ReadPDPacketDatas<PDGuildBuildingInstance>();
		foodWarehouseCollectionId = reader.ReadGuid();
		moralPointDate = reader.ReadDateTime();
		moralPoint = reader.ReadInt32();
		dailyGuildSupplySupportQuestStartDate = reader.ReadDateTime();
		dailyGuildSupplySupportQuestStartCount = reader.ReadInt32();
		dailyHuntingDonationDate = reader.ReadDateTime();
		dailyHuntingDonationCount = reader.ReadInt32();
		dailyObjectiveDate = reader.ReadDateTime();
		dailyObjectiveContentId = reader.ReadInt32();
		dailyObjectiveCompletionMemberCount = reader.ReadInt32();
		weeklyObjectiveDate = reader.ReadDateTime();
		weeklyObjectiveId = reader.ReadInt32();
		weeklyObjectiveCompletionMemberCount = reader.ReadInt32();
		lastBlessingBuffStartDate = reader.ReadDateTime();
		isBlessingBuffRunning = reader.ReadBoolean();
	}
}
