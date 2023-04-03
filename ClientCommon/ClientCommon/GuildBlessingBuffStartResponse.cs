namespace ClientCommon;

public class GuildBlessingBuffStartResponseBody : ResponseBody
{
	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
