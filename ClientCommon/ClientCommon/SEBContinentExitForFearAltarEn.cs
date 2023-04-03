namespace ClientCommon;

public class SEBContinentExitForFearAltarEnterEventBody : SEBServerEventBody
{
	public int stageId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stageId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stageId = reader.ReadInt32();
	}
}
