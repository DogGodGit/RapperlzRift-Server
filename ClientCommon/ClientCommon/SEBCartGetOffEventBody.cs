namespace ClientCommon;

public class SEBCartGetOffEventBody : SEBServerEventBody
{
	public long instanceId;

	public PDHero hero;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
		writer.Write(hero);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		hero = reader.ReadPDPacketData<PDHero>();
	}
}
