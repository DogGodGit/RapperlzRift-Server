using System;

namespace ClientCommon;

public class PDSimpleParty : PDPacketData
{
	public Guid id;

	public PDSimpleHero master;

	public int memberCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(master);
		writer.Write(memberCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		master = reader.ReadPDPacketData<PDSimpleHero>();
		memberCount = reader.ReadInt32();
	}
}
