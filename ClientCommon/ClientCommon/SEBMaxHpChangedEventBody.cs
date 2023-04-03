namespace ClientCommon;

public class SEBMaxHpChangedEventBody : SEBServerEventBody
{
	public int maxHp;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(maxHp);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		maxHp = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
