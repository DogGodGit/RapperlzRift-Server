namespace ClientCommon;

public class SupplySupportQuestCartChangeCommandBody : CommandBody
{
	public int wayPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wayPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wayPoint = reader.ReadInt32();
	}
}
