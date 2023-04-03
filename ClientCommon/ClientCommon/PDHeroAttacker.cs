using System;

namespace ClientCommon;

public class PDHeroAttacker : PDAttacker
{
	public Guid heroId;

	public string name;

	public int nationId;

	public override int type => 1;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
		writer.Write(name);
		writer.Write(nationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
	}
}
