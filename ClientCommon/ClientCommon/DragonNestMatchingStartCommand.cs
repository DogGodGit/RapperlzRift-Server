namespace ClientCommon;

public class DragonNestMatchingStartCommandBody : CommandBody
{
	public bool isPartyEntrance;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isPartyEntrance);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isPartyEntrance = reader.ReadBoolean();
	}
}
