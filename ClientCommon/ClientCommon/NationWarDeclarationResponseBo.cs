namespace ClientCommon;

public class NationWarDeclarationResponseBody : ResponseBody
{
	public PDNationWarDeclaration declaration;

	public int weeklyNationWarDeclarationCount;

	public long nationFund;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(declaration);
		writer.Write(weeklyNationWarDeclarationCount);
		writer.Write(nationFund);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		declaration = reader.ReadPDPacketData<PDNationWarDeclaration>();
		weeklyNationWarDeclarationCount = reader.ReadInt32();
		nationFund = reader.ReadInt64();
	}
}
