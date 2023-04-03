namespace ClientCommon;

public class AnkouTombMoneyBuffActivateResponseBody : ResponseBody
{
	public int maxHP;

	public long gold;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(gold);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		gold = reader.ReadInt64();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
