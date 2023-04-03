namespace ClientCommon;

public class PDAnkouTombMonsterInstance : PDMonsterInstance
{
	public int monsterType;

	public override MonsterInstanceType type => MonsterInstanceType.AnkouTombMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterType = reader.ReadInt32();
	}
}
