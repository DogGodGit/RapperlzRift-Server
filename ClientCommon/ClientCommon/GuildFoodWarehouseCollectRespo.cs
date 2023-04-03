using System;

namespace ClientCommon;

public class GuildFoodWarehouseCollectResponseBody : ResponseBody
{
	public Guid collectionId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(collectionId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		collectionId = reader.ReadGuid();
	}
}
