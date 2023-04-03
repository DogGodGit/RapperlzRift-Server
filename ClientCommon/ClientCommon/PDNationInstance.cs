namespace ClientCommon;

public class PDNationInstance : PDPacketData
{
	public int nationId;

	public long fund;

	public PDNationNoblesseInstance[] noblesseInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationId);
		writer.Write(fund);
		writer.Write(noblesseInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationId = reader.ReadInt32();
		fund = reader.ReadInt64();
		noblesseInsts = reader.ReadPDPacketDatas<PDNationNoblesseInstance>();
	}
}
