namespace ClientCommon;

public class PDHeroOrdealQuest : PDPacketData
{
	public int questNo;

	public bool completed;

	public PDHeroOrdealQuestSlot[] slots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questNo);
		writer.Write(completed);
		writer.Write(slots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questNo = reader.ReadInt32();
		completed = reader.ReadBoolean();
		slots = reader.ReadPDPacketDatas<PDHeroOrdealQuestSlot>();
	}
}
