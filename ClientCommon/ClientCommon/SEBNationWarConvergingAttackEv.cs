using System;

namespace ClientCommon;

public class SEBNationWarConvergingAttackEventBody : SEBServerEventBody
{
	public int targetMonsterArrangeId;

	public DateTime date;

	public int dailyNationWarConvergingAttackCount;

	public float nationWarConvergingAttackReminaingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetMonsterArrangeId);
		writer.Write(date);
		writer.Write(dailyNationWarConvergingAttackCount);
		writer.Write(nationWarConvergingAttackReminaingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetMonsterArrangeId = reader.ReadInt32();
		date = reader.ReadDateTime();
		dailyNationWarConvergingAttackCount = reader.ReadInt32();
		nationWarConvergingAttackReminaingCoolTime = reader.ReadInt32();
	}
}
