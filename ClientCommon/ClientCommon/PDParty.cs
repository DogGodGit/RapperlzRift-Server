using System;

namespace ClientCommon;

public class PDParty : PDPacketData
{
	public Guid id;

	public PDPartyMember[] members;

	public Guid masterId;

	public float callRemainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(members);
		writer.Write(masterId);
		writer.Write(callRemainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		members = reader.ReadPDPacketDatas<PDPartyMember>();
		masterId = reader.ReadGuid();
		callRemainingCoolTime = reader.ReadSingle();
	}
}
