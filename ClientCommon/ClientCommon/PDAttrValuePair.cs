namespace ClientCommon;

public class PDAttrValuePair : PDPacketData
{
	public int id;

	public int value;

	public PDAttrValuePair()
		: this(0, 0)
	{
	}

	public PDAttrValuePair(int nId, int nValue)
	{
		id = nId;
		value = nValue;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(value);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
		value = reader.ReadInt32();
	}
}
