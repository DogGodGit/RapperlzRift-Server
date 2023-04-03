namespace ClientCommon;

public class NationWarReviveResponseBody : ResponseBody
{
	public int revivalTargetContinentId;

	public int revivalTargetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(revivalTargetContinentId);
		writer.Write(revivalTargetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		revivalTargetContinentId = reader.ReadInt32();
		revivalTargetNationId = reader.ReadInt32();
	}
}
