using System;

namespace ClientCommon;

public class PDHeroTaskConsignment : PDPacketData
{
	public Guid instanceId;

	public int consignmentId;

	public int usedExpItemId;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(consignmentId);
		writer.Write(usedExpItemId);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadGuid();
		consignmentId = reader.ReadInt32();
		usedExpItemId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
