using System;

namespace ClientCommon;

public class CreatureComposeCommandBody : CommandBody
{
	public Guid mainInstanceId;

	public Guid materialInstanceId;

	public int[] protectedIndices;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(mainInstanceId);
		writer.Write(materialInstanceId);
		writer.Write(protectedIndices);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		mainInstanceId = reader.ReadGuid();
		materialInstanceId = reader.ReadGuid();
		protectedIndices = reader.ReadInts();
	}
}
