namespace ClientCommon;

public class PDHeroWingMemoryPieceSlot : PDPacketData
{
	public int index;

	public int accAttrValue;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(accAttrValue);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		accAttrValue = reader.ReadInt32();
	}
}
