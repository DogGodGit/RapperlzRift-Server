namespace ClientCommon;

public abstract class PDBooty : PDPacketData
{
	public const int kType_Item = 1;

	public abstract int type { get; }

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(type);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
	}

	public static PDBooty Create(int nType)
	{
		if (nType == 1)
		{
			return new PDItemBooty();
		}
		return null;
	}
}
