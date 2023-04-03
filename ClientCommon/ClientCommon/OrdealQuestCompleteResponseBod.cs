namespace ClientCommon;

public class OrdealQuestCompleteResponseBody : ResponseBody
{
	public PDHeroOrdealQuest nextQuest;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nextQuest);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nextQuest = reader.ReadPDPacketData<PDHeroOrdealQuest>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
