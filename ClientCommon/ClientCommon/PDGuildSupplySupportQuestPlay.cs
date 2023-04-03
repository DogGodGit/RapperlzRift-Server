namespace ClientCommon;

public class PDGuildSupplySupportQuestPlay : PDPacketData
{
	public long cartInstanceId;

	public int cartContinentId;

	public PDVector3 cartPosition;

	public float cartRotationY;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInstanceId);
		writer.Write(cartContinentId);
		writer.Write(cartPosition);
		writer.Write(cartRotationY);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInstanceId = reader.ReadInt64();
		cartContinentId = reader.ReadInt32();
		cartPosition = reader.ReadPDVector3();
		cartRotationY = reader.ReadSingle();
		remainingTime = reader.ReadSingle();
	}
}
