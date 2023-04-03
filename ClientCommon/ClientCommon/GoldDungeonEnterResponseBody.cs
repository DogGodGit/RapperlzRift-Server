using System;

namespace ClientCommon;

public class GoldDungeonEnterResponseBody : ResponseBody
{
	public DateTime date;

	public Guid placeInstanceId;

	public PDVector3 position;

	public float rotationY;

	public int hp;

	public int stamina;

	public int playCount;

	public int paidImmediateRevivalDailyCount;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(date);
		writer.Write(placeInstanceId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(hp);
		writer.Write(stamina);
		writer.Write(playCount);
		writer.Write(paidImmediateRevivalDailyCount);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		date = reader.ReadDateTime();
		placeInstanceId = reader.ReadGuid();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		hp = reader.ReadInt32();
		stamina = reader.ReadInt32();
		playCount = reader.ReadInt32();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
	}
}
