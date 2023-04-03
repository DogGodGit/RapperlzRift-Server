namespace ClientCommon;

public class SEBWarMemoryClearEventBody : SEBServerEventBody
{
	public PDWarMemoryRanking[] rankings;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHp;

	public int hp;

	public PDItemBooty[] rankingBooties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(rankings);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHp);
		writer.Write(hp);
		writer.Write(rankingBooties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		rankings = reader.ReadPDPacketDatas<PDWarMemoryRanking>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
		rankingBooties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
