using System.Collections.Generic;

namespace GameServer;

public struct SectorChangeInfo
{
	public HashSet<Sector> addedSectors;

	public HashSet<Sector> removedSectors;

	public HashSet<Sector> notChangedSectors;

	public SectorChangeInfo(HashSet<Sector> addedSectors, HashSet<Sector> removedSectors, HashSet<Sector> notChangedSectors)
	{
		this.addedSectors = addedSectors;
		this.removedSectors = removedSectors;
		this.notChangedSectors = notChangedSectors;
	}

	public static SectorChangeInfo NewEmpty()
	{
		return new SectorChangeInfo(new HashSet<Sector>(), new HashSet<Sector>(), new HashSet<Sector>());
	}
}
