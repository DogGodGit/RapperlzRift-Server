using System;

namespace ClientCommon;

public class PresentSendResponseBody : ResponseBody
{
	public DateTime weekStartDate;

	public int weeklyPresentContributionPoint;

	public int ownDia;

	public int unOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(weekStartDate);
		writer.Write(weeklyPresentContributionPoint);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		weekStartDate = reader.ReadDateTime();
		weeklyPresentContributionPoint = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
	}
}
