namespace ClientCommon;

public class WisdomTempleColorMatchingObjectInteractionStartCommandBody : CommandBody
{
	public long instanceId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(instanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
	}
}
public class WisdomTempleColorMatchingObjectInteractionStartResponseBody : ResponseBody
{
}
public class WisdomTempleColorMatchingObjectCheckCommandBody : CommandBody
{
	public int stepNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(stepNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		stepNo = reader.ReadInt32();
	}
}
public class WisdomTempleColorMatchingObjectCheckResponseBody : ResponseBody
{
	public int colorMatchingPoint;

	public PDWisdomTempleColorMatchingObjectInstance[] createdColorMatchingObjectInsts;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(colorMatchingPoint);
		writer.Write(createdColorMatchingObjectInsts);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		colorMatchingPoint = reader.ReadInt32();
		createdColorMatchingObjectInsts = reader.ReadPDPacketDatas<PDWisdomTempleColorMatchingObjectInstance>();
	}
}
