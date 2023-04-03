using System;

namespace ClientCommon;

public class NationWarCallResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyNationWarCallCount;

	public float nationWarCallRemainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyNationWarCallCount);
		writer.Write(nationWarCallRemainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyNationWarCallCount = reader.ReadInt32();
		nationWarCallRemainingCoolTime = reader.ReadSingle();
	}
}
