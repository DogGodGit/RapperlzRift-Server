using System;

namespace ClientCommon;

public class SEBGuildMoralPointChangedEventBody : SEBServerEventBody
{
	public DateTime date;

	public int moralPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(moralPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		moralPoint = reader.ReadInt32();
	}
}
