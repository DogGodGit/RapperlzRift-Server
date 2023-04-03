using System;

namespace ClientCommon;

public class CreatureInjectCommandBody : CommandBody
{
	public Guid instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
	}
}
