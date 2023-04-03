namespace ClientCommon;

public class WeekendRewardSelectCommandBody : CommandBody
{
	public int selectionNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(selectionNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		selectionNo = reader.ReadInt32();
	}
}
