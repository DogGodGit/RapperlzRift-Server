namespace ClientCommon;

public class PDHeroBountyHunterQuest : PDPacketData
{
	public int questId;

	public int itemGrade;

	public int progressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
		writer.Write(itemGrade);
		writer.Write(progressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadInt32();
		itemGrade = reader.ReadInt32();
		progressCount = reader.ReadInt32();
	}
}
