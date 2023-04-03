using System;

namespace ClientCommon;

public class PDNationAllianceApplication : PDPacketData
{
	public Guid id;

	public int nationId;

	public int targetNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(nationId);
		writer.Write(targetNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		nationId = reader.ReadInt32();
		targetNationId = reader.ReadInt32();
	}
}
