namespace ClientCommon;

public abstract class PDDropObject : PDPacketData
{
	public const int kType_MainGear = 1;

	public const int kTYpe_Item = 2;

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

	public static PDDropObject Create(int nType)
	{
		return nType switch
		{
			1 => new PDMainGearDropObject(), 
			2 => new PDItemDropObject(), 
			_ => null, 
		};
	}
}
