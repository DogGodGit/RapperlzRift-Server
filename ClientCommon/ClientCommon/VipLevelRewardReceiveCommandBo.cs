namespace ClientCommon;

public class VipLevelRewardReceiveCommandBody : CommandBody
{
	public int vipLevel;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(vipLevel);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		vipLevel = reader.ReadInt32();
	}
}
