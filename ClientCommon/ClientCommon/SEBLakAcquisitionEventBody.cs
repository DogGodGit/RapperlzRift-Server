namespace ClientCommon;

public class SEBLakAcquisitionEventBody : SEBServerEventBody
{
	public int lak;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(lak);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		lak = reader.ReadInt32();
	}
}
