namespace ClientCommon;

public class GuildWeeklyObjectiveSetCommandBody : CommandBody
{
	public int objectiveId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(objectiveId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		objectiveId = reader.ReadInt32();
	}
}
