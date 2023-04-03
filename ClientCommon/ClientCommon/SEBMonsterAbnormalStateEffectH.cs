namespace ClientCommon;

public class SEBMonsterAbnormalStateEffectHitEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public int hp;

	public long abnormalStateEffectInstanceId;

	public int damage;

	public int hpDamage;

	public long[] removedAbnormalStateEffects;

	public PDAttacker attacker;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(hp);
		writer.Write(abnormalStateEffectInstanceId);
		writer.Write(damage);
		writer.Write(hpDamage);
		writer.Write(removedAbnormalStateEffects);
		writer.Write(attacker);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		hp = reader.ReadInt32();
		abnormalStateEffectInstanceId = reader.ReadInt64();
		damage = reader.ReadInt32();
		hpDamage = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
		attacker = reader.ReadPDAttacker();
	}
}
