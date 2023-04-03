namespace ClientCommon;

public class PDWarMemoryMonsterInstance : PDMonsterInstance
{
	public int monsterType;

	public override MonsterInstanceType type => MonsterInstanceType.WarMemoryMonster;

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
