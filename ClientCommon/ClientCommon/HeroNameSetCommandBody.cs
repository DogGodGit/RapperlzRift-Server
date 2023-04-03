using System;

namespace ClientCommon;

public class HeroNameSetCommandBody : CommandBody
{
	public Guid heroId;

	public string name;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
	}
}
