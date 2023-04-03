using System;

namespace ClientCommon;

public class CEBMoveEndEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public Guid heroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
		writer.Write(heroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
		heroId = reader.ReadGuid();
	}
}
