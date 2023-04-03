namespace ClientCommon;

public class PDHeroTitle : PDPacketData
{
	public int titleId;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(titleId);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		titleId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
