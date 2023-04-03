using System;

namespace ClientCommon;

public class SEBHeroRevivedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int hp;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(hp);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		hp = reader.ReadInt32();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
