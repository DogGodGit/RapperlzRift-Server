namespace ClientCommon;

public class PDEliteDungeonMonsterInstance : PDMonsterInstance
{
	public int eliteMonsterId;

	public override MonsterInstanceType type => MonsterInstanceType.EliteDungeonMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(eliteMonsterId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		eliteMonsterId = reader.ReadInt32();
	}
}
