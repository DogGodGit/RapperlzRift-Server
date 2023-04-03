using System;

namespace ClientCommon;

public class SEBGuildAppointedEventBody : SEBServerEventBody
{
	public Guid appointerId;

	public string appointerName;

	public int appointerGrade;

	public Guid appointeeId;

	public string appointeeName;

	public int appointeeGrade;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(appointerId);
		writer.Write(appointerName);
		writer.Write(appointerGrade);
		writer.Write(appointeeId);
		writer.Write(appointeeName);
		writer.Write(appointeeGrade);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		appointerId = reader.ReadGuid();
		appointerName = reader.ReadString();
		appointerGrade = reader.ReadInt32();
		appointeeId = reader.ReadGuid();
		appointeeName = reader.ReadString();
		appointeeGrade = reader.ReadInt32();
	}
}
