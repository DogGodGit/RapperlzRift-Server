namespace ClientCommon;

public class PDHeroOpen7DayEventProgressCount : PDPacketData
{
	public int type;

	public int accProgressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(type);
		writer.Write(accProgressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		type = reader.ReadInt32();
		accProgressCount = reader.ReadInt32();
	}
}
