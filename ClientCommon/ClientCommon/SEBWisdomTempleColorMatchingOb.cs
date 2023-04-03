namespace ClientCommon;

public class SEBWisdomTempleColorMatchingObjectInteractionFinishedEventBody : SEBServerEventBody
{
	public PDWisdomTempleColorMatchingObjectInstance colorMatchingObjectInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(colorMatchingObjectInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		colorMatchingObjectInst = reader.ReadPDPacketData<PDWisdomTempleColorMatchingObjectInstance>();
	}
}
public class SEBWisdomTempleColorMatchingObjectInteractionCancelEventBody : SEBServerEventBody
{
}
