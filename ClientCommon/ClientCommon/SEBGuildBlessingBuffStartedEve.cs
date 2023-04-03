using System;

namespace ClientCommon;

public class SEBGuildBlessingBuffStartedEventBody : SEBServerEventBody
{
	public DateTime blessingBuffStartDate;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(blessingBuffStartDate);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		blessingBuffStartDate = reader.ReadDateTime();
	}
}
