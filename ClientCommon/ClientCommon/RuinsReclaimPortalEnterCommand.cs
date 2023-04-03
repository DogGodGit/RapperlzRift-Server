namespace ClientCommon;

public class RuinsReclaimPortalEnterCommandBody : CommandBody
{
	public int portalId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(portalId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		portalId = reader.ReadInt32();
	}
}
