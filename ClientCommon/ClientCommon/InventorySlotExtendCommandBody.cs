namespace ClientCommon;

public class InventorySlotExtendCommandBody : CommandBody
{
	public int extendSlotCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(extendSlotCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		extendSlotCount = reader.ReadInt32();
	}
}
