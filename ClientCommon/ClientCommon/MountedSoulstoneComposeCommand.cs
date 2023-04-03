namespace ClientCommon;

public class MountedSoulstoneComposeCommandBody : CommandBody
{
	public int subGearId;

	public int slotIndex;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
		writer.Write(slotIndex);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
		slotIndex = reader.ReadInt32();
	}
}
