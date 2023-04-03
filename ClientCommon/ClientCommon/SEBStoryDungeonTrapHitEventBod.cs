namespace ClientCommon;

public class SEBStoryDungeonTrapHitEventBody : SEBServerEventBody
{
	public bool isImmortal;

	public int hp;

	public int damage;

	public int hpDamage;

	public PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isImmortal);
		writer.Write(hp);
		writer.Write(damage);
		writer.Write(hpDamage);
		writer.Write(changedAbnormalStateEffectDamageAbsorbShields);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isImmortal = reader.ReadBoolean();
		hp = reader.ReadInt32();
		damage = reader.ReadInt32();
		hpDamage = reader.ReadInt32();
		changedAbnormalStateEffectDamageAbsorbShields = reader.ReadPDPacketDatas<PDAbnormalStateEffectDamageAbsorbShield>();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
