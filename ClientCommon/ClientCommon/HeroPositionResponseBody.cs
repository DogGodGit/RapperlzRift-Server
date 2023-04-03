using System;

namespace ClientCommon;

public class HeroPositionResponseBody : ResponseBody
{
	public bool isLoggedIn;

	public Guid placeInstanceId;

	public int locationId;

	public int locationParam;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isLoggedIn);
		writer.Write(placeInstanceId);
		writer.Write(locationId);
		writer.Write(locationParam);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isLoggedIn = reader.ReadBoolean();
		placeInstanceId = reader.ReadGuid();
		locationId = reader.ReadInt32();
		locationParam = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
