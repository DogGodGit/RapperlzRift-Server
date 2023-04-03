namespace ClientCommon;

public class PremiumStarEssenseItemUseCommandBody : CommandBody
{
	public int slotIndex;

	public int useCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(slotIndex);
		writer.Write(useCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		slotIndex = reader.ReadInt32();
		useCount = reader.ReadInt32();
	}
}
