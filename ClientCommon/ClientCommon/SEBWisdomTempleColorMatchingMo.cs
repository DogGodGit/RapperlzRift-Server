namespace ClientCommon;

public class SEBWisdomTempleColorMatchingMonsterKillEventBody : SEBServerEventBody
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
public class SEBWisdomTempleColorMatchingMonsterCreatedEventBody : SEBServerEventBody
{
	public PDWisdomTempleColorMatchingMonsterInstance monsterInst;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterInst);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterInst = reader.ReadPDMonsterInstance<PDWisdomTempleColorMatchingMonsterInstance>();
	}
}
