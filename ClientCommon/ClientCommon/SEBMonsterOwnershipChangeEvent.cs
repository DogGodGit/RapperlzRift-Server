using System;

namespace ClientCommon;

public class SEBMonsterOwnershipChangeEventBody : SEBServerEventBody
{
	public long instanceId;

	public Guid ownerId;

	public MonsterOwnerType ownerType;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(ownerId);
		writer.WriteEnumInt(ownerType);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		ownerId = reader.ReadGuid();
		ownerType = reader.ReadEnumInt<MonsterOwnerType>();
	}
}
