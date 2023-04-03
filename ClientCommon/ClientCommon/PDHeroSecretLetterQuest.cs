namespace ClientCommon;

public class PDHeroSecretLetterQuest : PDPacketData
{
	public int targetNationId;

	public int pickCount;

	public int pickedLetterGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetNationId);
		writer.Write(pickCount);
		writer.Write(pickedLetterGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetNationId = reader.ReadInt32();
		pickCount = reader.ReadInt32();
		pickedLetterGrade = reader.ReadInt32();
	}
}
