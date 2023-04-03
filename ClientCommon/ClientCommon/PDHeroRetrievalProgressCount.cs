using System;

namespace ClientCommon;

public class PDHeroRetrievalProgressCount : PDPacketData
{
	public DateTime date;

	public int retrievalId;

	public int prorgressCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(retrievalId);
		writer.Write(prorgressCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		retrievalId = reader.ReadInt32();
		prorgressCount = reader.ReadInt32();
	}
}
