namespace ClientCommon;

public abstract class PDWarehouseObject : PDPacketData
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

	public static PDWarehouseObject Create(int nType)
	{
		return nType switch
		{
			1 => new PDMainGearWarehouseObject(), 
			2 => new PDSubGearWarehouseObject(), 
			3 => new PDItemWarehouseObject(), 
			4 => new PDMountGearWarehouseObject(), 
			_ => null, 
		};
	}
}
