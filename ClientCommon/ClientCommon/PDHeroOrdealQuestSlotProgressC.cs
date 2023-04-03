namespace ClientCommon;

public class PDHeroOrdealQuestSlotProgressCount : PDPacketData
{
	public int index;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
