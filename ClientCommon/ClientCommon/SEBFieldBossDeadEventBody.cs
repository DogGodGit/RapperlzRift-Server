namespace ClientCommon;

public class SEBFieldBossDeadEventBody : SEBServerEventBody
{
	public int fieldBossId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(fieldBossId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		fieldBossId = reader.ReadInt32();
	}
}
