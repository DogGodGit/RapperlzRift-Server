namespace ClientCommon;

public class PDHeroCreatureBaseAttr : PDPacketData
{
	public int attrId;

	public int baseValue;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(attrId);
		writer.Write(baseValue);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		attrId = reader.ReadInt32();
		baseValue = reader.ReadInt32();
	}
}
