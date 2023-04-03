namespace ClientCommon;

public class SEBAnkouTombMoneyBuffFinishedEventBody : SEBServerEventBody
{
	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
