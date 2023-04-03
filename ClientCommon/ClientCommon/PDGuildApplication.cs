using System;

namespace ClientCommon;

public class PDGuildApplication : PDPacketData
{
	public Guid id;

	public Guid heroId;

	public string heroName;

	public int heroJobId;

	public int heroLevel;

	public int heroVipLevel;

	public long heroBattlePower;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(heroId);
		writer.Write(heroName);
		writer.Write(heroJobId);
		writer.Write(heroLevel);
		writer.Write(heroVipLevel);
		writer.Write(heroBattlePower);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		heroId = reader.ReadGuid();
		heroName = reader.ReadString();
		heroJobId = reader.ReadInt32();
		heroLevel = reader.ReadInt32();
		heroVipLevel = reader.ReadInt32();
		heroBattlePower = reader.ReadInt64();
	}
}
