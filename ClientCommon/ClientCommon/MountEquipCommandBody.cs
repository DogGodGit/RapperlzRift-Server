namespace ClientCommon;

public class MountEquipCommandBody : CommandBody
{
	public int mountId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mountId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mountId = reader.ReadInt32();
	}
}
