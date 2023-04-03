using System;

namespace ClientCommon;

public class MountGearUnequipCommandBody : CommandBody
{
	public Guid heroMountGearId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMountGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMountGearId = reader.ReadGuid();
	}
}
