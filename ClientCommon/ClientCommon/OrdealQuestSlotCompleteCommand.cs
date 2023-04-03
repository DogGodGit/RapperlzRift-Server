namespace ClientCommon;

public class OrdealQuestSlotCompleteCommandBody : CommandBody
{
	public int index;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
	}
}
