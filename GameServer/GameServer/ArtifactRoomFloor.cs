using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomFloor
{
	private ArtifactRoom m_artifactRoom;

	private int m_nFloor;

	private string m_sNameKey;

	private int m_nRequiredHeroLevel;

	private long m_lnRecommendBattlePower;

	private int m_nSweepDuration;

	private int m_nSweepDia;

	private ItemReward m_itemReward;

	private List<ArtifactRoomMonsterArrange> m_arranges = new List<ArtifactRoomMonsterArrange>();

	public ArtifactRoom artifactRoom => m_artifactRoom;

	public int floor => m_nFloor;

	public string nameKey => m_sNameKey;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public long recommendBattlePower => m_lnRecommendBattlePower;

	public int sweepDuration => m_nSweepDuration;

	public int sweepDia => m_nSweepDia;

	public ItemReward itemReward => m_itemReward;

	public List<ArtifactRoomMonsterArrange> arranges => m_arranges;

	public ArtifactRoomFloor(ArtifactRoom artifactRoom)
	{
		m_artifactRoom = artifactRoom;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nFloor = Convert.ToInt32(dr["floor"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		m_lnRecommendBattlePower = Convert.ToInt64(dr["recommendBattlePower"]);
		m_nSweepDuration = Convert.ToInt32(dr["sweepDuration"]);
		m_nSweepDia = Convert.ToInt32(dr["sweepDia"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nFloor = " + m_nFloor + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nFloor = " + m_nFloor + ", lnItemRewardId = " + lnItemRewardId);
		}
	}

	public void AddArrange(ArtifactRoomMonsterArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arranges.Add(arrange);
	}

	public ArtifactRoomMonsterArrange GetArrange(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_arranges.Count)
		{
			return null;
		}
		return m_arranges[nIndex];
	}

	public static int GetTotalSweepDia(int nStartFloor, int nEndFloor)
	{
		int nTotalDia = 0;
		foreach (ArtifactRoomFloor floor in Resource.instance.artifactRoom.floors)
		{
			if (floor.floor <= nEndFloor)
			{
				if (floor.floor >= nStartFloor)
				{
					nTotalDia += floor.sweepDia;
				}
				continue;
			}
			return nTotalDia;
		}
		return nTotalDia;
	}
}
