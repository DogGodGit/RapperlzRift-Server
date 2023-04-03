using System;

namespace ClientCommon;

public class PDNationWarDeclaration : PDPacketData
{
	public Guid declarationId;

	public int nationId;

	public int targetNationId;

	public DateTimeOffset time;

	public int status;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(declarationId);
		writer.Write(nationId);
		writer.Write(targetNationId);
		writer.Write(time);
		writer.Write(status);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		declarationId = reader.ReadGuid();
		nationId = reader.ReadInt32();
		targetNationId = reader.ReadInt32();
		time = reader.ReadDateTimeOffset();
		status = reader.ReadInt32();
	}
}
