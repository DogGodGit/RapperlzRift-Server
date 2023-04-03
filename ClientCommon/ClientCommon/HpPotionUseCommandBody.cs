namespace ClientCommon;

public class HpPotionUseCommandBody : CommandBody
{
	public int inventorySlotIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(inventorySlotIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		inventorySlotIndex = reader.ReadInt32();
	}
}
