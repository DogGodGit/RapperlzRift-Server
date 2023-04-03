namespace ClientCommon;

public class PDMonsterAttacker : PDAttacker
{
	public long monsterInstanceId;

	public int monsterId;

	public override int type => 2;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(monsterId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		monsterId = reader.ReadInt32();
	}
}
