using System;

namespace ClientCommon;

public class PDNationWarHistory : PDPacketData
{
	public DateTime date;

	public int offenseNationId;

	public int defenseNationId;

	public int winNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(offenseNationId);
		writer.Write(defenseNationId);
		writer.Write(winNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		offenseNationId = reader.ReadInt32();
		defenseNationId = reader.ReadInt32();
		winNationId = reader.ReadInt32();
	}
}
