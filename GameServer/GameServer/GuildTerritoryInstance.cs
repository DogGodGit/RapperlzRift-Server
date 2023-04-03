using System;

namespace GameServer;

public class GuildTerritoryInstance : Place
{
	private Guild m_guild;

	private GuildTerritory m_territory;

	public override PlaceType placeType => PlaceType.GuildTerritory;

	public override Location location => m_territory;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_territory.mapRect;

	public override bool interestManaged => true;

	public override bool ownershipManaged => true;

	public override bool battleEnabled => true;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => true;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public Guild guild => m_guild;

	public GuildTerritory territory => m_territory;

	public void Init(Guild guild)
	{
		if (guild == null)
		{
			throw new ArgumentNullException("guild");
		}
		m_guild = guild;
		m_territory = Resource.instance.guildTerritory;
		InitPlace();
	}
}
