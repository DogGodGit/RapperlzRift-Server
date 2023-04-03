using System;

namespace ClientCommon;

public class SEBTrueHeroQuestTauntedEventBody : SEBServerEventBody
{
	public int nationId;

	public Guid heroId;

	public string heroName;

	public int continentId;

	public PDVector3 position;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(nationId);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(continentId);
		writer.Write(position);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		nationId = reader.ReadInt32();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		continentId = reader.ReadInt32();
		position = reader.ReadPDVector3();
	}
}
