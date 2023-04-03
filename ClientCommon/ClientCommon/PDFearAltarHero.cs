using System;

namespace ClientCommon;

public class PDFearAltarHero : PDPacketData
{
	public Guid heroId;

	public string name;

	public int jobId;

	public int level;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(jobId);
		writer.Write(level);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
	}
}
