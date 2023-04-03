using System;

namespace ClientCommon;

public class NationWarConvergingAttackResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyNationWarConvergingAttackCount;

	public float nationWarConvergingAttackReminaingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyNationWarConvergingAttackCount);
		writer.Write(nationWarConvergingAttackReminaingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyNationWarConvergingAttackCount = reader.ReadInt32();
		nationWarConvergingAttackReminaingCoolTime = reader.ReadSingle();
	}
}
