namespace ClientCommon;

public class TestResponseBody : ResponseBody
{
	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
	}
}
