using System;

namespace ClientCommon;

public class PresentReplyCommandBody : CommandBody
{
	public Guid targetHeroid;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetHeroid);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetHeroid = reader.ReadGuid();
	}
}
