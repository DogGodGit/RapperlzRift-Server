using System;

namespace ClientCommon;

public class StaminaBuyResponseBody : ResponseBody
{
	public DateTime date;

	public int dailyBuyCount;

	public int stamina;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(dailyBuyCount);
		writer.Write(stamina);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		dailyBuyCount = reader.ReadInt32();
		stamina = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
