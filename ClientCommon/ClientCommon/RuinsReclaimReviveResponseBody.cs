using System;

namespace ClientCommon;

public class RuinsReclaimReviveResponseBody : ResponseBody
{
	public int hp;

	public PDVector3 position;

	public float rotationY;

	public DateTime date;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(hp);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(date);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		hp = reader.ReadInt32();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		date = reader.ReadDateTime();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
