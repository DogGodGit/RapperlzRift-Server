using System;

namespace ClientCommon;

public class PDContinentObjectInstance : PDPacketData
{
	public long instanceId;

	public int arrangeNo;

	public Guid interactionHeroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(arrangeNo);
		writer.Write(interactionHeroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		arrangeNo = reader.ReadInt32();
		interactionHeroId = reader.ReadGuid();
	}
}
