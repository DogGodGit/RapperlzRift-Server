using System;

namespace ClientCommon;

public class SEBNationWarCallEventBody : SEBServerEventBody
{
	public Guid callerId;

	public string callerName;

	public int callerNoblesseId;

	public DateTime date;

	public int dailyNationWarCallCount;

	public float nationWarCallRemainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(callerId);
		writer.Write(callerName);
		writer.Write(callerNoblesseId);
		writer.Write(date);
		writer.Write(dailyNationWarCallCount);
		writer.Write(nationWarCallRemainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		callerId = reader.ReadGuid();
		callerName = reader.ReadString();
		callerNoblesseId = reader.ReadInt32();
		date = reader.ReadDateTime();
		dailyNationWarCallCount = reader.ReadInt32();
		nationWarCallRemainingCoolTime = reader.ReadSingle();
	}
}
