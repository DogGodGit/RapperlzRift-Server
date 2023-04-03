namespace ClientCommon;

public class PDBiographyQuestDungeonMonsterInstance : PDMonsterInstance
{
	public int arrangeKey;

	public override MonsterInstanceType type => MonsterInstanceType.BiographQuestDungeonMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(arrangeKey);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		arrangeKey = reader.ReadInt32();
	}
}
