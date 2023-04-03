using System;

namespace ClientCommon;

public class ContinentEnterForNationWarTransmissionCommandBody : CommandBody
{
}
public class ContinentEnterForNationWarTransmissionResponseBody : ResponseBody
{
	public PDContinentEntranceInfo entranceInfo;

	public DateTime date;

	public int nationWarFreeTransmissionCount;

	public int nationWarPaidTransmissionCount;

	public int ownDIa;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(entranceInfo);
		writer.Write(date);
		writer.Write(nationWarFreeTransmissionCount);
		writer.Write(nationWarPaidTransmissionCount);
		writer.Write(ownDIa);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		entranceInfo = reader.ReadPDPacketData<PDContinentEntranceInfo>();
		date = reader.ReadDateTime();
		nationWarFreeTransmissionCount = reader.ReadInt32();
		nationWarPaidTransmissionCount = reader.ReadInt32();
		ownDIa = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
