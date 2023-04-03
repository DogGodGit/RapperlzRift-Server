namespace ClientCommon;

public class MainQuestAcceptCommandBody : CommandBody
{
	public int mainQuestNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainQuestNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainQuestNo = reader.ReadInt32();
	}
}
