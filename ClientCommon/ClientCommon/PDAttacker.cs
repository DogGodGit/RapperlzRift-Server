namespace ClientCommon;

public abstract class PDAttacker : PDPacketData
{
	public const int kType_Hero = 1;

	public const int kType_Monster = 2;

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

	public static PDAttacker Create(int nType)
	{
		return nType switch
		{
			1 => new PDHeroAttacker(), 
			2 => new PDMonsterAttacker(), 
			_ => null, 
		};
	}
}
