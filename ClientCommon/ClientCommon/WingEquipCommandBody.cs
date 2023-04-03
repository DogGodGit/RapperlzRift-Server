namespace ClientCommon;

public class WingEquipCommandBody : CommandBody
{
	public int wingId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(wingId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		wingId = reader.ReadInt32();
	}
}
