namespace ClientCommon;

public class PDHitResult : PDPacketData
{
	public PDAttacker attacker;

	public int skillId;

	public int chainSkillId;

	public int hitId;

	public bool isCritical;

	public bool isPenetration;

	public bool isBlocked;

	public bool isImmortal;

	public int hp;

	public int damage;

	public int hpDamage;

	public PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(attacker);
		writer.Write(skillId);
		writer.Write(chainSkillId);
		writer.Write(hitId);
		writer.Write(isCritical);
		writer.Write(isPenetration);
		writer.Write(isBlocked);
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
		attacker = reader.ReadPDAttacker();
		skillId = reader.ReadInt32();
		chainSkillId = reader.ReadInt32();
		hitId = reader.ReadInt32();
		isCritical = reader.ReadBoolean();
		isPenetration = reader.ReadBoolean();
		isBlocked = reader.ReadBoolean();
		isImmortal = reader.ReadBoolean();
		hp = reader.ReadInt32();
		damage = reader.ReadInt32();
		hpDamage = reader.ReadInt32();
		changedAbnormalStateEffectDamageAbsorbShields = reader.ReadPDPacketDatas<PDAbnormalStateEffectDamageAbsorbShield>();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
