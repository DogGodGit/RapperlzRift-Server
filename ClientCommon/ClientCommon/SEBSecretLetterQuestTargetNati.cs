namespace ClientCommon;

public class SEBSecretLetterQuestTargetNationChangedEventBody : SEBServerEventBody
{
	public int targetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetNationId = reader.ReadInt32();
	}
}
