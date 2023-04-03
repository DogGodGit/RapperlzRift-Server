using System;

namespace ClientCommon;

public class PDHeroHitTarget : PDHitTarget
{
	public Guid heroId;

	public override int type => 1;

	public PDHeroHitTarget()
	{
	}

	public PDHeroHitTarget(Guid heroId)
	{
		this.heroId = heroId;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroId = reader.ReadGuid();
	}
}
