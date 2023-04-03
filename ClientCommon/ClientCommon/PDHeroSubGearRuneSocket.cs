namespace ClientCommon;

public class PDHeroSubGearRuneSocket : PDPacketData
{
	public int index;

	public int itemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(itemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		itemId = reader.ReadInt32();
	}
}
