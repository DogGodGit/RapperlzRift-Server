using System;

namespace ClientCommon;

public class SEBDailyConsumeEventProgressEventBody : SEBServerEventBody
{
	public DateTime date;

	public int accDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(accDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		accDia = reader.ReadInt32();
	}
}
