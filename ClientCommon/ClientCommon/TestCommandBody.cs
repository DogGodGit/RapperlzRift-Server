namespace ClientCommon;

public class TestCommandBody : CommandBody
{
	public int no;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt32();
	}
}
