namespace ClientCommon;

public class PDHeroMainQuest : PDPacketData
{
	public int no;

	public int progressCount;

	public bool completed;

	public long cartInstanceId;

	public int cartContinentId;

	public PDVector3 cartPosition;

	public float cartRotationY;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(no);
		writer.Write(progressCount);
		writer.Write(completed);
		writer.Write(cartInstanceId);
		writer.Write(cartContinentId);
		writer.Write(cartPosition);
		writer.Write(cartRotationY);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		no = reader.ReadInt32();
		progressCount = reader.ReadInt32();
		completed = reader.ReadBoolean();
		cartInstanceId = reader.ReadInt64();
		cartContinentId = reader.ReadInt32();
		cartPosition = reader.ReadPDVector3();
		cartRotationY = reader.ReadSingle();
	}
}
