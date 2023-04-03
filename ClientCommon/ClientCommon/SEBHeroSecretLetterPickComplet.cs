using System;

namespace ClientCommon;

public class SEBHeroSecretLetterPickCompletedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int pickedLetterGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(pickedLetterGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		pickedLetterGrade = reader.ReadInt32();
	}
}
