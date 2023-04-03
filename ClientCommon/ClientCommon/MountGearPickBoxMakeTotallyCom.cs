namespace ClientCommon;

public class MountGearPickBoxMakeTotallyCommandBody : CommandBody
{
	public int mountGearPickBoxItemId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mountGearPickBoxItemId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mountGearPickBoxItemId = reader.ReadInt32();
	}
}
