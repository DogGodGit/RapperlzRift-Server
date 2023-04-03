namespace ClientCommon;

public class ItemComposeTotallyCommandBody : CommandBody
{
	public int materialItemId;

	public bool owned;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(materialItemId);
		writer.Write(owned);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		materialItemId = reader.ReadInt32();
		owned = reader.ReadBoolean();
	}
}
