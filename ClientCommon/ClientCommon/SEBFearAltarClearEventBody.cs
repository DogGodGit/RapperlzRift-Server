namespace ClientCommon;

public class SEBFearAltarClearEventBody : SEBServerEventBody
{
	public PDFearAltarHero[] clearedHeroes;

	public long acquiredExp;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(clearedHeroes);
		writer.Write(acquiredExp);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		clearedHeroes = reader.ReadPDPacketDatas<PDFearAltarHero>();
		acquiredExp = reader.ReadInt64();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
