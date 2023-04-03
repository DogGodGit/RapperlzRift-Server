namespace ClientCommon;

public class CostumeEffectApplyCommandBody : CommandBody
{
	public int costumeId;

	public int costumeEffectId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(costumeId);
		writer.Write(costumeEffectId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		costumeId = reader.ReadInt32();
		costumeEffectId = reader.ReadInt32();
	}
}
