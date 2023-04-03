namespace ClientCommon;

public class PDHeroMainGearRefinement : PDPacketData
{
	public int turn;

	public PDHeroMainGearRefinementAttr[] attrs;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(turn);
		writer.Write(attrs);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		turn = reader.ReadInt32();
		attrs = reader.ReadPDPacketDatas<PDHeroMainGearRefinementAttr>();
	}
}
