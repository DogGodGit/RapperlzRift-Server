using System;

namespace GameServer;

public class DisputeContinentInstance : ContinentInstance
{
	public override PlaceType placeType => PlaceType.DisputeContinent;

	public override int locationParam => 0;

	public override int nationId => 0;

	public override bool pvpEnabled => true;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => true;

	public override bool isExpScrollBuffEnabled => true;

	public override bool isExpLevelPenaltyEnabled => true;

	public override bool isWorldLevelExpBuffEnabled => true;

	public void InitDisputeContinent(Continent continent)
	{
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		InitContinent(continent);
	}

	public override bool IsSame(int nTargetContinentId, int nTargetNationId)
	{
		return m_continent.id == nTargetContinentId;
	}
}
