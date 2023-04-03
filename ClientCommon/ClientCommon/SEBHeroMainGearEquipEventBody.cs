using System;

namespace ClientCommon;

public class SEBHeroMainGearEquipEventBody : SEBServerEventBody
{
	public Guid heroId;

	public PDHeroMainGear heroMainGear;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(heroMainGear);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		heroMainGear = reader.ReadPDPacketData<PDHeroMainGear>();
	}
}
