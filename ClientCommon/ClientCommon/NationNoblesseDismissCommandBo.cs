namespace ClientCommon;

public class NationNoblesseDismissCommandBody : CommandBody
{
	public int targetNoblesseId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(targetNoblesseId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		targetNoblesseId = reader.ReadInt32();
	}
}
