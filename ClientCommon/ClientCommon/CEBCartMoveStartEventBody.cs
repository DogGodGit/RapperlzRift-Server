using System;

namespace ClientCommon;

public class CEBCartMoveStartEventBody : CEBClientEventBody
{
	public Guid placeInstanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(placeInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		placeInstanceId = reader.ReadGuid();
	}
}
