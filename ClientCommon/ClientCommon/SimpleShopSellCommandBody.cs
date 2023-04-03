namespace ClientCommon;

public class SimpleShopSellCommandBody : CommandBody
{
	public int[] slotIndices;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(slotIndices);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		slotIndices = reader.ReadInts();
	}
}
