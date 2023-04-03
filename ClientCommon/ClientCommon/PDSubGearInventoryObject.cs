namespace ClientCommon;

public class PDSubGearInventoryObject : PDInventoryObject
{
	public int subGearId;

	public override int type => 2;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
	}
}
