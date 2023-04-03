namespace ClientCommon;

public class PDHeroHolyWarQuest : PDPacketData
{
	public int killCount;

	public float remainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(killCount);
		writer.Write(remainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		killCount = reader.ReadInt32();
		remainingTime = reader.ReadSingle();
	}
}
