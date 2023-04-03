using System;

namespace ClientCommon;

public class SEBMonsterMentalHitEventBody : SEBServerEventBody
{
	public long monsterInstanceId;

	public Guid attackerId;

	public Guid tamerId;

	public int skillId;

	public int mentalStrength;

	public int damage;

	public int mentalStrengthDamage;

	public long[] removedAbnormalStateEffects;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInstanceId);
		writer.Write(attackerId);
		writer.Write(tamerId);
		writer.Write(skillId);
		writer.Write(mentalStrength);
		writer.Write(damage);
		writer.Write(mentalStrengthDamage);
		writer.Write(removedAbnormalStateEffects);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInstanceId = reader.ReadInt64();
		attackerId = reader.ReadGuid();
		tamerId = reader.ReadGuid();
		skillId = reader.ReadInt32();
		mentalStrength = reader.ReadInt32();
		damage = reader.ReadInt32();
		mentalStrengthDamage = reader.ReadInt32();
		removedAbnormalStateEffects = reader.ReadLongs();
	}
}
