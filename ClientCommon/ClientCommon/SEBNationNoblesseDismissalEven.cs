namespace ClientCommon;

public class SEBNationNoblesseDismissalEventBody : SEBServerEventBody
{
	public int nationId;

	public int noblesseId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationId);
		writer.Write(noblesseId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationId = reader.ReadInt32();
		noblesseId = reader.ReadInt32();
	}
}
