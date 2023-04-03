using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildMission : IPickEntry
{
	private int m_nId;

	private int m_nPoint;

	private GuildMissionType m_type;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Npc m_targetNpc;

	private ContinentObject m_targetContinentObject;

	private Monster m_targetMonster;

	private MonsterArrange m_targetSummonMonsterArrange;

	private float m_fTargetSummonMonsterRadius;

	private int m_nTargetSummonMonsterKillLimitTime;

	private int m_nTargetCount;

	private GuildContributionPointReward m_contributionPointReward;

	private GuildFundReward m_fundReward;

	private GuildBuildingPointReward m_buildingPointReawrd;

	public int id => m_nId;

	public int point => m_nPoint;

	public GuildMissionType type => m_type;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Npc targetNpc => m_targetNpc;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public Monster targetMonster => m_targetMonster;

	public MonsterArrange targetSummonMonsterArrange => m_targetSummonMonsterArrange;

	public float targetSummonMonsterRadius => m_fTargetSummonMonsterRadius;

	public int targetSummonMonsterKillLimitTime => m_nTargetSummonMonsterKillLimitTime;

	public int targetCount => m_nTargetCount;

	public GuildContributionPointReward contributionPointReward => m_contributionPointReward;

	public int contributionPointRewardValue
	{
		get
		{
			if (m_contributionPointReward == null)
			{
				return 0;
			}
			return m_contributionPointReward.value;
		}
	}

	public GuildFundReward fundReward => m_fundReward;

	public int fundRewardValue
	{
		get
		{
			if (m_fundReward == null)
			{
				return 0;
			}
			return m_fundReward.value;
		}
	}

	public GuildBuildingPointReward buildingPointReawrd => m_buildingPointReawrd;

	public int buildingPointRewardValue
	{
		get
		{
			if (m_buildingPointReawrd == null)
			{
				return 0;
			}
			return m_buildingPointReawrd.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
		int nTpye = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(GuildMissionType), nTpye))
		{
			SFLogUtil.Warn(GetType(), "길드미션타입이 유효하지 않습니다. m_nId = " + m_nId + ", nTpye = " + nTpye);
		}
		m_type = (GuildMissionType)nTpye;
		switch (m_type)
		{
		case GuildMissionType.Find:
		{
			int nTargetNpcId = Convert.ToInt32(dr["targetNpcId"]);
			m_targetNpc = Resource.instance.GetNpc(nTargetNpcId);
			if (m_targetNpc == null)
			{
				SFLogUtil.Warn(GetType(), "목표NPC가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetNpcId = " + nTargetNpcId);
			}
			break;
		}
		case GuildMissionType.Hunt:
		{
			int nContinentId = Convert.ToInt32(dr["targetContinentId"]);
			m_targetContinent = Resource.instance.GetContinent(nContinentId);
			if (m_targetContinent == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
			}
			m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
			m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
			m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
			m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
			if (m_fTargetRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), "목표 반경이 유효하지 않습니다. m_nId = " + m_nId + "m_fTargetRadius = " + m_fTargetRadius);
			}
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표 몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case GuildMissionType.Summon:
		{
			long lnTargetSummonMonsterArrangeId = Convert.ToInt64(dr["targetSummonMonsterArrangeId"]);
			m_targetSummonMonsterArrange = Resource.instance.GetMonsterArrange(lnTargetSummonMonsterArrangeId);
			if (m_targetSummonMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "목표소환몬스터배치가 존재하지 않습니다. m_nId = " + m_nId + ", lnTargetSummonMonsterArrangeId = " + lnTargetSummonMonsterArrangeId);
			}
			m_fTargetSummonMonsterRadius = Convert.ToInt64(dr["targetSummonMonsterRadius"]);
			if (m_fTargetSummonMonsterRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), "목표소환몬스터반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fTargetRadius = " + m_fTargetRadius);
			}
			m_nTargetSummonMonsterKillLimitTime = Convert.ToInt32(dr["targetSummonMonsterKillLimitTime"]);
			if (m_nTargetSummonMonsterKillLimitTime <= 0)
			{
				SFLogUtil.Warn(GetType(), "목표소환몬스터킬제한시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetSummonMonsterKillLimitTime = " + m_nTargetSummonMonsterKillLimitTime);
			}
			break;
		}
		case GuildMissionType.Announce:
		{
			int nContinentId2 = Convert.ToInt32(dr["targetContinentId"]);
			m_targetContinent = Resource.instance.GetContinent(nContinentId2);
			if (m_targetContinent == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId2);
			}
			m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
			m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
			m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
			m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
			if (m_fTargetRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), "목표 반경이 유효하지 않습니다. m_nId = " + m_nId + "m_fTargetRadius = " + m_fTargetRadius);
			}
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표대륙오브젝트가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표 횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
		long lnContributionPointRewardId = Convert.ToInt64(dr["guildContributionPointRewardId"]);
		m_contributionPointReward = Resource.instance.GetGuildContributionPointReward(lnContributionPointRewardId);
		if (m_contributionPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "길드공헌도보상이 존재하지 않습니다. m_nId = " + m_nId + " lnContributionPointRewardId = " + lnContributionPointRewardId);
		}
		long lnFundRewardId = Convert.ToInt64(dr["guildFundRewardId"]);
		m_fundReward = Resource.instance.GetGuildFundReward(lnFundRewardId);
		if (m_fundReward == null)
		{
			SFLogUtil.Warn(GetType(), "길드자금보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnFundRewardId = " + lnFundRewardId);
		}
		long lnBuildingPointReawrdId = Convert.ToInt64(dr["guildBuildingPointRewardId"]);
		m_buildingPointReawrd = Resource.instance.GetGuildBuildingPointReward(lnBuildingPointReawrdId);
		if (m_buildingPointReawrd == null)
		{
			SFLogUtil.Warn(GetType(), "길드건설도보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnBuildingPointReawrdId = " + lnBuildingPointReawrdId);
		}
	}

	public Vector3 SelectSummonMonsterPosition(Vector3 position)
	{
		return Util.SelectPoint(position, m_fTargetSummonMonsterRadius);
	}
}
