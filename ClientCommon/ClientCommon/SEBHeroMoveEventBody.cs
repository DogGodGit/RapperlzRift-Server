using System;

namespace ClientCommon;

public class SEBHeroMoveEventBody : SEBServerEventBody
{
	public Guid heroId;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
