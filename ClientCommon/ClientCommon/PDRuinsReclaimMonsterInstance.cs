namespace ClientCommon;

public class PDRuinsReclaimMonsterInstance : PDMonsterInstance
{
	public int monsterType;

	public override MonsterInstanceType type => MonsterInstanceType.RuinsReclaimMonster;

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
