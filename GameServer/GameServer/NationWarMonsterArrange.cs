using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarMonsterArrange
{
	public const int kType_CommanderInChief = 1;

	public const int kType_Wizard = 2;

	public const int kType_Angel = 3;

	public const int kType_Dragon = 4;

	public const int kType_Rock = 5;

	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	public const int kCommanderInChiefFinalDamagePenaltyRate = 10000;

	public const int kWizardDamagePenaltyDecreaseRate = 5000;

	public const int kLogType_Free = 1;

	public const int kLogType_Paid = 2;

	private NationWar m_nationWar;

	private int m_nId;

	private MonsterArrange m_monsterArrange;

	private int m_nType;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private int m_nNationWarNpcId;

	private int m_nRegenTime;

	private Vector3 m_transmissionPosition = Vector3.zero;

	private float m_fTransmissionRadius;

	private int m_nTransmissionYRotationType;

	private float m_fTransmissionYRotation;

	public NationWar nationWar => m_nationWar;

	public int id => m_nId;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int type => m_nType;

	public Continent continent => m_continent;

	public int continentId => m_continent.id;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public int nationWarNpcId => m_nNationWarNpcId;

	public int regenTime => m_nRegenTime;

	public Vector3 transmissionPosition => m_transmissionPosition;

	public float transmissionRadius => m_fTransmissionRadius;

	public int transmissionYRotationType => m_nTransmissionYRotationType;

	public float transmissionYRotation => m_fTransmissionYRotation;

	public NationWarMonsterArrange(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nId = Convert.ToInt32(dr["arrangeId"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = res.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else if (lnMonsterArrangeId < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		int nContinentId = Convert.ToInt32(dr["continentId"]);
		if (nContinentId > 0)
		{
			m_continent = res.GetContinent(nContinentId);
			if (m_continent == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nNationWarNpcId = Convert.ToInt32(dr["nationWarNpcId"]);
		if (m_nNationWarNpcId > 0)
		{
			if (m_nationWar.GetNpc(m_nNationWarNpcId) == null)
			{
				SFLogUtil.Warn(GetType(), "국가전NPC가 존재하지 않습니다. m_nId = " + m_nId + ", m_nNationWarNpcId = " + m_nNationWarNpcId);
			}
		}
		else if (m_nNationWarNpcId < 0)
		{
			SFLogUtil.Warn(GetType(), "국가전NPCID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nNationWarNpcId = " + m_nNationWarNpcId);
		}
		m_nRegenTime = Convert.ToInt32(dr["regenTime"]);
		m_transmissionPosition.x = Convert.ToSingle(dr["transmissionXPosition"]);
		m_transmissionPosition.y = Convert.ToSingle(dr["transmissionYPosition"]);
		m_transmissionPosition.z = Convert.ToSingle(dr["transmissionZPosition"]);
		m_fTransmissionRadius = Convert.ToSingle(dr["transmissionRadius"]);
		m_nTransmissionYRotationType = Convert.ToInt32(dr["transmissionYRotationType"]);
		if (!IsDefinedYRotationTYpe(m_nTransmissionYRotationType))
		{
			SFLogUtil.Warn(GetType(), "전송방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTransmissionYRotationType = " + m_nTransmissionYRotationType);
		}
		m_fTransmissionYRotation = Convert.ToSingle(dr["transmissionYRotation"]);
	}

	public Vector3 SelectTransmissionPosition()
	{
		return Util.SelectPoint(m_transmissionPosition, m_fTransmissionRadius);
	}

	public float SelectTransmissionRotationY()
	{
		if (m_nTransmissionYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fTransmissionYRotation);
		}
		return m_fTransmissionYRotation;
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1 && nType != 2 && nType != 3 && nType != 4)
		{
			return nType == 5;
		}
		return true;
	}

	public static bool IsDefinedYRotationTYpe(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
