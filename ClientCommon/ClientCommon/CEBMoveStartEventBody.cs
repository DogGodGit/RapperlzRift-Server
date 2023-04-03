using System;

namespace ClientCommon;

public class CEBMoveStartEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public Guid heroId;

	public bool isWalking;

	public bool isManualMoving;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroId);
		writer.Write(isWalking);
		writer.Write(isManualMoving);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroId = reader.ReadGuid();
		isWalking = reader.ReadBoolean();
		isManualMoving = reader.ReadBoolean();
	}
}
