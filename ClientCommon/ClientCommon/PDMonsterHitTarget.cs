namespace ClientCommon;

public class PDMonsterHitTarget : PDHitTarget
{
	public long monsterInstanceId;

	public override int type => 2;

	public PDMonsterHitTarget()
	{
	}

	public PDMonsterHitTarget(long lnMonsterInstanceId)
	{
		monsterInstanceId = lnMonsterInstanceId;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
	}
}
