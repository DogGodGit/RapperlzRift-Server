namespace ClientCommon;

public class PDFullHeroSubGear : PDPacketData
{
	public int subGearId;

	public int level;

	public int quality;

	public bool equipped;

	public PDHeroSubGearSoulstoneSocket[] equippedSoulstoneSockets;

	public PDHeroSubGearRuneSocket[] equippedRuneSockets;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(subGearId);
		writer.Write(level);
		writer.Write(quality);
		writer.Write(equipped);
		writer.Write(equippedSoulstoneSockets);
		writer.Write(equippedRuneSockets);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		subGearId = reader.ReadInt32();
		level = reader.ReadInt32();
		quality = reader.ReadInt32();
		equipped = reader.ReadBoolean();
		equippedSoulstoneSockets = reader.ReadPDPacketDatas<PDHeroSubGearSoulstoneSocket>();
		equippedRuneSockets = reader.ReadPDPacketDatas<PDHeroSubGearRuneSocket>();
	}
}
