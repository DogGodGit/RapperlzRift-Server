namespace ClientCommon;

public class PDHeroMysteryBoxQuest : PDPacketData
{
	public int pickCount;

	public int pickedBoxGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(pickCount);
		writer.Write(pickedBoxGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		pickCount = reader.ReadInt32();
		pickedBoxGrade = reader.ReadInt32();
	}
}
