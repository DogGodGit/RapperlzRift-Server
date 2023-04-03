namespace ClientCommon;

public class PDNationWarMonsterInstance : PDMonsterInstance
{
	public int arrangeId;

	public override MonsterInstanceType type => MonsterInstanceType.NationWarMonster;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(arrangeId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		arrangeId = reader.ReadInt32();
	}
}
