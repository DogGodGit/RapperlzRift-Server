namespace ClientCommon;

public class PDHeroOrdealQuestSlot : PDPacketData
{
	public int index;

	public int missionNo;

	public int progressCount;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(missionNo);
		writer.Write(progressCount);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		missionNo = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
