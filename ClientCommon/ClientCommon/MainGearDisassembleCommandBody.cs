using System;

namespace ClientCommon;

public class MainGearDisassembleCommandBody : CommandBody
{
	public Guid[] targetHeroMainGearIds;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetHeroMainGearIds);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetHeroMainGearIds = reader.ReadGuids();
	}
}
