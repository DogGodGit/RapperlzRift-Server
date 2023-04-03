namespace ClientCommon;

public class BlessingQuestBlessingSendCommandBody : CommandBody
{
	public long questId;

	public int blessingId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(questId);
		writer.Write(blessingId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		questId = reader.ReadInt64();
		blessingId = reader.ReadInt32();
	}
}
