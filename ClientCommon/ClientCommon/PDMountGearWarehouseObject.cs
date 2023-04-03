using System;

namespace ClientCommon;

public class PDMountGearWarehouseObject : PDWarehouseObject
{
	public Guid heroMountGearId;

	public override int type => 4;

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
