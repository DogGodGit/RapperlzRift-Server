using System;

namespace ClientCommon;

public class PDMainGearInventoryObject : PDInventoryObject
{
	public Guid heroMainGearId;

	public override int type => 1;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMainGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMainGearId = reader.ReadGuid();
	}
}
