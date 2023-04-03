namespace ClientCommon;

public class SEBCartEnterEventBody : SEBServerEventBody
{
	public PDCartInstance cartInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInst = reader.ReadPDCartInstance();
	}
}
