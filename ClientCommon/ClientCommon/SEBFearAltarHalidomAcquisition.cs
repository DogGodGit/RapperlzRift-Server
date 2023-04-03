using System;

namespace ClientCommon;

public class SEBFearAltarHalidomAcquisitionEventBody : SEBServerEventBody
{
	public DateTime weekDateTime;

	public int halidomId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(weekDateTime);
		writer.Write(halidomId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		weekDateTime = reader.ReadDateTime();
		halidomId = reader.ReadInt32();
	}
}
