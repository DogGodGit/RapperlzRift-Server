using System;

namespace ClientCommon;

public class MainGearRefinementApplyCommandBody : CommandBody
{
	public Guid heroMainGearId;

	public int turn;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMainGearId);
		writer.Write(turn);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMainGearId = reader.ReadGuid();
		turn = reader.ReadInt32();
	}
}
