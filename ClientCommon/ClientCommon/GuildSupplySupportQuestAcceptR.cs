using System;

namespace ClientCommon;

public class GuildSupplySupportQuestAcceptResponseBody : ResponseBody
{
	public PDGuildSupplySupportQuestCartInstance cartInst;

	public float remainingTime;

	public DateTime date;

	public int dailyGuildSupplySupportQuestStartCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInst);
		writer.Write(remainingTime);
		writer.Write(date);
		writer.Write(dailyGuildSupplySupportQuestStartCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInst = reader.ReadPDCartInstance<PDGuildSupplySupportQuestCartInstance>();
		remainingTime = reader.ReadSingle();
		date = reader.ReadDateTime();
		dailyGuildSupplySupportQuestStartCount = reader.ReadInt32();
	}
}
