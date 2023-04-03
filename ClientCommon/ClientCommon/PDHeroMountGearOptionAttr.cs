namespace ClientCommon;

public class PDHeroMountGearOptionAttr : PDPacketData
{
	public int index;

	public int grade;

	public int attrId;

	public long attrValueId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(index);
		writer.Write(grade);
		writer.Write(attrId);
		writer.Write(attrValueId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		index = reader.ReadInt32();
		grade = reader.ReadInt32();
		attrId = reader.ReadInt32();
		attrValueId = reader.ReadInt64();
	}
}
