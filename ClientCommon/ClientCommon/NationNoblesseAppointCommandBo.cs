using System;

namespace ClientCommon;

public class NationNoblesseAppointCommandBody : CommandBody
{
	public int targetNoblesseId;

	public Guid targetHeroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetNoblesseId);
		writer.Write(targetHeroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetNoblesseId = reader.ReadInt32();
		targetHeroId = reader.ReadGuid();
	}
}
