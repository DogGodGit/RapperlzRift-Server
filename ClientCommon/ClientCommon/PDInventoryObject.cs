namespace ClientCommon;

public abstract class PDInventoryObject : PDPacketData
{
	public const int kType_MainGear = 1;

	public const int kType_SubGear = 2;

	public const int kType_Item = 3;

	public const int kType_MountGear = 4;

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

	public static PDInventoryObject Create(int nType)
	{
		return nType switch
		{
			1 => new PDMainGearInventoryObject(), 
			2 => new PDSubGearInventoryObject(), 
			3 => new PDItemInventoryObject(), 
			4 => new PDMountGearInventoryObject(), 
			_ => null, 
		};
	}
}
