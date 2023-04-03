namespace ClientCommon;

public class PDGoldDungeonMonsterInstance : PDMonsterInstance
{
	public int stepNo;

	public int activationWaveNo;

	public override MonsterInstanceType type => MonsterInstanceType.GoldDungeonMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
		writer.Write(activationWaveNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
		activationWaveNo = reader.ReadInt32();
	}
}
