using System;

namespace ClientCommon;

public class MountGearRefineCommandBody : CommandBody
{
	public Guid heroMountGearId;

	public int optionAttrIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMountGearId);
		writer.Write(optionAttrIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMountGearId = reader.ReadGuid();
		optionAttrIndex = reader.ReadInt32();
	}
}
