namespace ClientCommon;

public class SEBSecretLetterPickCompletedEventBody : SEBServerEventBody
{
	public int pickCount;

	public int pickedLetterGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(pickCount);
		writer.Write(pickedLetterGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		pickCount = reader.ReadInt32();
		pickedLetterGrade = reader.ReadInt32();
	}
}
