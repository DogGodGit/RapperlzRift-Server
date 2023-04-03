namespace ClientCommon;

public class NationWarHistoryResponseBody : ResponseBody
{
	public PDNationWarHistory[] histories;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(histories);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		histories = reader.ReadPDPacketDatas<PDNationWarHistory>();
	}
}
