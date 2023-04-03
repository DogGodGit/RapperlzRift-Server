using System;

namespace ClientCommon;

public class PDNationCall : PDPacketData
{
	public long id;

	public Guid callerId;

	public string callerName;

	public int callerNoblesseId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(callerId);
		writer.Write(callerName);
		writer.Write(callerNoblesseId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt64();
		callerId = reader.ReadGuid();
		callerName = reader.ReadString();
		callerNoblesseId = reader.ReadInt32();
	}
}
