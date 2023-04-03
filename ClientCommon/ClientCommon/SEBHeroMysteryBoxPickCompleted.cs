using System;

namespace ClientCommon;

public class SEBHeroMysteryBoxPickCompletedEventBody : SEBServerEventBody
{
	public Guid heroId;

	public int pickedBoxGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(pickedBoxGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		pickedBoxGrade = reader.ReadInt32();
	}
}
