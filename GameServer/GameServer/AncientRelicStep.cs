using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelicStep
{
	public const int kType_MonsterHunt = 1;

	public const int kType_PointAcquisition = 2;

	private AncientRelic m_ancientRelic;

	private int m_nNo;

	private int m_nType;

	private string m_sTargetTitleKey;

	private string m_sTargetContentKey;

	private string m_sGuideTitleKey;

	private string m_sGuideContentKey;

	private int m_nTargetPoint;

	private List<AncientRelicStepRoute> m_routes = new List<AncientRelicStepRoute>();

	private Dictionary<int, AncientRelicStepRewardPoolCollection> m_rewardPoolCollections = new Dictionary<int, AncientRelicStepRewardPoolCollection>();

	private List<AncientRelicStepWave> m_waves = new List<AncientRelicStepWave>();

	public AncientRelic ancientRelic => m_ancientRelic;

	public int no => m_nNo;

	public int type => m_nType;

	public string targetTitleKey => m_sTargetTitleKey;

	public string targetContentKey => m_sTargetContentKey;

	public string guideTitleKey => m_sGuideTitleKey;

	public string guideContentKey => m_sGuideContentKey;

	public int targetPoint => m_nTargetPoint;

	public int waveCount => m_waves.Count;

	public AncientRelicStep(AncientRelic ancientRelic)
	{
		m_ancientRelic = ancientRelic;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["step"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_sTargetTitleKey = Convert.ToString(dr["targetTitleKey"]);
		m_sTargetContentKey = Convert.ToString(dr["targetContentKey"]);
		m_sGuideTitleKey = Convert.ToString(dr["guideTitleKey"]);
		m_sGuideContentKey = Convert.ToString(dr["guideContentKey"]);
		m_nTargetPoint = Convert.ToInt32(dr["targetPoint"]);
	}

	public void AddRoute(AncientRelicStepRoute route)
	{
		if (route == null)
		{
			throw new ArgumentNullException("route");
		}
		m_routes.Add(route);
	}

	public AncientRelicStepRoute GetRoute(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_routes.Count)
		{
			return null;
		}
		return m_routes[nIndex];
	}

	public AncientRelicStepRewardPoolCollection GetRewardPoolCollection(int nLevel)
	{
		if (!m_rewardPoolCollections.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public AncientRelicStepRewardPoolCollection GetOrCreateRewardPoolCollection(int nLevel)
	{
		AncientRelicStepRewardPoolCollection rewardPoolCollection = GetRewardPoolCollection(nLevel);
		if (rewardPoolCollection == null)
		{
			rewardPoolCollection = new AncientRelicStepRewardPoolCollection(this, nLevel);
			m_rewardPoolCollections.Add(rewardPoolCollection.level, rewardPoolCollection);
		}
		return rewardPoolCollection;
	}

	public void AddWave(AncientRelicStepWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public AncientRelicStepWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
