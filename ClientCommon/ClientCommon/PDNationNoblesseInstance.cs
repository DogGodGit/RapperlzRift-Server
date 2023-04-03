using System;

namespace ClientCommon;

public class PDNationNoblesseInstance : PDPacketData
{
	public int noblesseId;

	public Guid heroId;

	public string heroName;

	public int heroJobId;

	public DateTime appointmentDate;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(noblesseId);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(heroJobId);
		writer.Write(appointmentDate);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		noblesseId = reader.ReadInt32();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		heroJobId = reader.ReadInt32();
		appointmentDate = reader.ReadDateTime();
	}
}
