using System;

namespace ClientCommon;

public class CEBMoveEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public Guid heroId;

	public PDVector3 position;

	public float rotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroId);
		writer.Write(position);
		writer.Write(rotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
	}
}
