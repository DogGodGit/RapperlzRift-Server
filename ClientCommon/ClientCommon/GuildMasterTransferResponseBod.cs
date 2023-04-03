namespace ClientCommon;

public class GuildMasterTransferResponseBody : ResponseBody
{
	public int memberGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(memberGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		memberGrade = reader.ReadInt32();
	}
}
