using System;

namespace ClientCommon;

public class PDHeroTradeShipBestRecord : PDPacketData
{
	public Guid heroId;

	public string heroName;

	public int heroJobId;

	public int heroNationId;

	public int difficulty;

	public int point;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(heroJobId);
		writer.Write(heroNationId);
		writer.Write(difficulty);
		writer.Write(point);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		heroJobId = reader.ReadInt32();
		heroNationId = reader.ReadInt32();
		difficulty = reader.ReadInt32();
		point = reader.ReadInt32();
	}
}
