using System;

namespace ClientCommon;

public class MainGearEquipCommandBody : CommandBody
{
	public Guid heroMainGearId;

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
