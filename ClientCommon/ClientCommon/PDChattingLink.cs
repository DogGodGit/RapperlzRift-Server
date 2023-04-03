namespace ClientCommon;

public abstract class PDChattingLink : PDPacketData
{
	public const int kType_MainGear = 1;

	public const int kType_SubGear = 2;

	public const int kType_MountGear = 3;

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

	public static PDChattingLink Create(int nType)
	{
		return nType switch
		{
			1 => new PDMainGearChattingLink(), 
			2 => new PDSubGearChattingLink(), 
			3 => new PDMountGearChattingLink(), 
			_ => null, 
		};
	}
}
