using System;

namespace ClientCommon;

public class MainGearRefineCommandBody : CommandBody
{
	public Guid heroMainGearId;

	public bool isSingleRefinement;

	public int[] protectedIndices;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMainGearId);
		writer.Write(isSingleRefinement);
		writer.Write(protectedIndices);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMainGearId = reader.ReadGuid();
		isSingleRefinement = reader.ReadBoolean();
		protectedIndices = reader.ReadInts();
	}
}
