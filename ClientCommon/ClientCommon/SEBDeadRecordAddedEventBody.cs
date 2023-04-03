using System;

namespace ClientCommon;

public class SEBDeadRecordAddedEventBody : SEBServerEventBody
{
	public PDDeadRecord record;

	public Guid removedRecordId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(record);
		writer.Write(removedRecordId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		record = reader.ReadPDPacketData<PDDeadRecord>();
		removedRecordId = reader.ReadGuid();
	}
}
