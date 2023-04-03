namespace ClientCommon;

public class PDHeroSupplySupportQuest : PDPacketData
{
	public long cartInstanceId;

	public int cartContinentId;

	public PDVector3 cartPosition;

	public float cartRotationY;

	public int orderId;

	public int cartId;

	public float remainingTime;

	public int[] visitedWayPoints;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInstanceId);
		writer.Write(cartContinentId);
		writer.Write(cartPosition);
		writer.Write(cartRotationY);
		writer.Write(orderId);
		writer.Write(cartId);
		writer.Write(remainingTime);
		writer.Write(visitedWayPoints);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInstanceId = reader.ReadInt64();
		cartContinentId = reader.ReadInt32();
		cartPosition = reader.ReadPDVector3();
		cartRotationY = reader.ReadSingle();
		orderId = reader.ReadInt32();
		cartId = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
		visitedWayPoints = reader.ReadInts();
	}
}
