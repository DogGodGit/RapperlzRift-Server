namespace ClientCommon;

public class DistortionScrollUseCommandBody : CommandBody
{
	public int slotIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(slotIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		slotIndex = reader.ReadInt32();
	}
}
