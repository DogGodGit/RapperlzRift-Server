namespace ClientCommon;

public class SEBNationWarDeclarationEventBody : SEBServerEventBody
{
	public PDNationWarDeclaration nationWarDeclaration;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationWarDeclaration);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationWarDeclaration = reader.ReadPDPacketData<PDNationWarDeclaration>();
	}
}
