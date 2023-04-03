using System;

namespace ClientCommon;

public class CreatureRearCommandBody : CommandBody
{
	public Guid instanceId;

	public int itemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(itemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		itemId = reader.ReadInt32();
	}
}
