namespace ClientCommon;

public class SEBMonsterHpRestoredEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		hp = reader.ReadInt32();
	}
}
