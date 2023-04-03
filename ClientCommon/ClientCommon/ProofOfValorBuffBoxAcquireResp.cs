namespace ClientCommon;

public class ProofOfValorBuffBoxAcquireResponseBody : ResponseBody
{
	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hp = reader.ReadInt32();
	}
}
