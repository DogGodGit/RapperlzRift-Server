using System;

namespace ClientCommon;

public class SEBPartyMasterChangedEventBody : SEBServerEventBody
{
	public Guid masterId;

	public float callRemainingCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(masterId);
		writer.Write(callRemainingCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		masterId = reader.ReadGuid();
		callRemainingCoolTime = reader.ReadSingle();
	}
}
