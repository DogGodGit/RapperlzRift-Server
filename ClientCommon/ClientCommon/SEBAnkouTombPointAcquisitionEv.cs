namespace ClientCommon;

public class SEBAnkouTombPointAcquisitionEventBody : SEBServerEventBody
{
	public int point;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(point);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		point = reader.ReadInt32();
	}
}
