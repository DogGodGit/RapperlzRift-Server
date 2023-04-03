using System;

namespace ClientCommon;

public class PDNationAlliance : PDPacketData
{
	public Guid id;

	public int[] nationIds;

	public float allianceRenounceAvailableRemainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(nationIds);
		writer.Write(allianceRenounceAvailableRemainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		nationIds = reader.ReadInts();
		allianceRenounceAvailableRemainingTime = reader.ReadSingle();
	}
}
