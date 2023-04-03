using System;

namespace ClientCommon;

public class PDHeroBlessingQuest : PDPacketData
{
	public long id;

	public Guid targetHeroId;

	public string targetName;

	public int blessingTargetLevelId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(targetHeroId);
		writer.Write(targetName);
		writer.Write(blessingTargetLevelId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt64();
		targetHeroId = reader.ReadGuid();
		targetName = reader.ReadString();
		blessingTargetLevelId = reader.ReadInt32();
	}
}
