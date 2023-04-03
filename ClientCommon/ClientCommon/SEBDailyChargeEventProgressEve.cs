using System;

namespace ClientCommon;

public class SEBDailyChargeEventProgressEventBody : SEBServerEventBody
{
	public DateTime date;

	public int accUnOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(accUnOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		accUnOwnDia = reader.ReadInt32();
	}
}
