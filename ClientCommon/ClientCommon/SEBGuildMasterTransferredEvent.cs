using System;

namespace ClientCommon;

public class SEBGuildMasterTransferredEventBody : SEBServerEventBody
{
	public Guid transfererId;

	public string transfererName;

	public Guid transfereeId;

	public string transfereeName;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(transfererId);
		writer.Write(transfererName);
		writer.Write(transfereeId);
		writer.Write(transfereeName);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		transfererId = reader.ReadGuid();
		transfererName = reader.ReadString();
		transfereeId = reader.ReadGuid();
		transfereeName = reader.ReadString();
	}
}
