namespace ClientCommon;

public abstract class PDHitTarget : PDPacketData
{
	public const int kType_Hero = 1;

	public const int kType_Monster = 2;

	public const int kType_Cart = 3;

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

	public static PDHitTarget Create(int nType)
	{
		return nType switch
		{
			1 => new PDHeroHitTarget(), 
			2 => new PDMonsterHitTarget(), 
			3 => new PDCartHitTarget(), 
			_ => null, 
		};
	}
}
