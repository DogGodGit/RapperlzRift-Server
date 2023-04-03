using System;

namespace ClientCommon;

public class PresentSendCommandBody : CommandBody
{
	public Guid targetHeroid;

	public int presentId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetHeroid);
		writer.Write(presentId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetHeroid = reader.ReadGuid();
		presentId = reader.ReadInt32();
	}
}
