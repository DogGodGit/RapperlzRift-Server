namespace ClientCommon;

public class GuildFarmQuestCompleteResponseBody : ResponseBody
{
	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public int guildContributionPoint;

	public int totalGuildContributionPoint;

	public PDInventorySlot[] changedInventorySlots;

	public int giBuildingPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(guildContributionPoint);
		writer.Write(totalGuildContributionPoint);
		writer.Write(changedInventorySlots);
		writer.Write(giBuildingPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		guildContributionPoint = reader.ReadInt32();
		totalGuildContributionPoint = reader.ReadInt32();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		giBuildingPoint = reader.ReadInt32();
	}
}
