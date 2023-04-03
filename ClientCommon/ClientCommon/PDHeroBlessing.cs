using System;

namespace ClientCommon;

public class PDHeroBlessing : PDPacketData
{
	public long instanceId;

	public int blessingId;

	public int blessingTargetLevelId;

	public Guid senderHeroId;

	public string senderName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(blessingId);
		writer.Write(blessingTargetLevelId);
		writer.Write(senderHeroId);
		writer.Write(senderName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		blessingId = reader.ReadInt32();
		blessingTargetLevelId = reader.ReadInt32();
		senderHeroId = reader.ReadGuid();
		senderName = reader.ReadString();
	}
}
