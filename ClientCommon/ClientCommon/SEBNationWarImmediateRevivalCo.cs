namespace ClientCommon;

public class SEBNationWarImmediateRevivalCountUpdatedEventBody : SEBServerEventBody
{
	public int immediateRevivalCount;

	public int accImmediateRevivalCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(immediateRevivalCount);
		writer.Write(accImmediateRevivalCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		immediateRevivalCount = reader.ReadInt32();
		accImmediateRevivalCount = reader.ReadInt32();
	}
}
