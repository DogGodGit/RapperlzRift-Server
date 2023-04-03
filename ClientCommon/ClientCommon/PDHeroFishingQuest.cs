namespace ClientCommon;

public class PDHeroFishingQuest : PDPacketData
{
	public int baitItemId;

	public int castingCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(baitItemId);
		writer.Write(castingCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		baitItemId = reader.ReadInt32();
		castingCount = reader.ReadInt32();
	}
}
