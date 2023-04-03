using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimStep
{
	public const int kType_Move = 1;

	public const int kType_Interaction = 2;

	public const int kType_Wave = 3;

	private RuinsReclaim m_ruinsReclaim;

	private int m_nNo;

	private int m_nType;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private int m_nRemoveObstacleId;

	private RuinsReclaimPortal m_activationPortal;

	private int m_nDeactivationPortalId;

	private RuinsReclaimRevivalPoint m_revivalPoint;

	private List<RuinsReclaimObjectArrange> m_objectArranges = new List<RuinsReclaimObjectArrange>();

	private List<RuinsReclaimStepReward> m_rewards = new List<RuinsReclaimStepReward>();

	private List<RuinsReclaimStepWave> m_waves = new List<RuinsReclaimStepWave>();

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int no => m_nNo;

	public int type => m_nType;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public int removeObstacleId => m_nRemoveObstacleId;

	public RuinsReclaimPortal activationPortal => m_activationPortal;

	public int deactivationPortalId => m_nDeactivationPortalId;

	public RuinsReclaimRevivalPoint revivalPoint => m_revivalPoint;

	public List<RuinsReclaimObjectArrange> objectArranges => m_objectArranges;

	public List<RuinsReclaimStepReward> rewards => m_rewards;

	public int waveCount => m_waves.Count;

	public RuinsReclaimStep(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표반지름이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
		}
		m_nRemoveObstacleId = Convert.ToInt32(dr["removeObstacleId"]);
		int nActivationPortalId = Convert.ToInt32(dr["activationPortalId"]);
		if (nActivationPortalId > 0)
		{
			m_activationPortal = m_ruinsReclaim.GetPortal(nActivationPortalId);
			if (m_activationPortal == null)
			{
				SFLogUtil.Warn(GetType(), "활성화포탈이 존재하지 않습니다. m_nNo = " + m_nNo + ", nActivationPortalId = " + nActivationPortalId);
			}
		}
		else if (nActivationPortalId < 0)
		{
			SFLogUtil.Warn(GetType(), "활성화포탈ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", nActivationPortalId = " + nActivationPortalId);
		}
		m_nDeactivationPortalId = Convert.ToInt32(dr["deactivationPortalId"]);
		if (m_nDeactivationPortalId > 0)
		{
			if (m_ruinsReclaim.GetPortal(m_nDeactivationPortalId) == null)
			{
				SFLogUtil.Warn(GetType(), "비활성화포탈이 존재하지 않습니다. m_nNo = " + m_nNo + ", m_nDeactivationPortalId = " + m_nDeactivationPortalId);
			}
		}
		else if (m_nDeactivationPortalId < 0)
		{
			SFLogUtil.Warn(GetType(), "비활성화포탈ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nDeactivationPortalId = " + m_nDeactivationPortalId);
		}
		if (nActivationPortalId != 0 && m_nDeactivationPortalId != 0 && nActivationPortalId == m_nDeactivationPortalId)
		{
			SFLogUtil.Warn(GetType(), "활성화포탈ID와 비활성화포탈ID가 같습니다. nActivationPortalId = " + nActivationPortalId + ", m_nDeactivationPortalId = " + m_nDeactivationPortalId);
		}
		int nRevivalPointId = Convert.ToInt32(dr["revivalPointId"]);
		if (nRevivalPointId > 0)
		{
			m_revivalPoint = m_ruinsReclaim.GetRevivalPoint(nRevivalPointId);
			if (m_revivalPoint == null)
			{
				SFLogUtil.Warn(GetType(), "부활지점이 존재하지 않습니다. m_nNo = " + m_nNo + ", nRevivalPointId = " + nRevivalPointId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "부활지점이 유효하지 않습니다. m_nNo = " + m_nNo + ", nRevivalPointId = " + nRevivalPointId);
		}
	}

	public void AddObjectArrange(RuinsReclaimObjectArrange objectArrange)
	{
		if (objectArrange == null)
		{
			throw new ArgumentNullException("objectArrange");
		}
		m_objectArranges.Add(objectArrange);
	}

	public void AddReward(RuinsReclaimStepReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public void AddWave(RuinsReclaimStepWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public RuinsReclaimStepWave GetWave(int nWaveNo)
	{
		int nIndex = nWaveNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public bool ContainsTargetPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius, position);
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1 && nType != 2)
		{
			return nType == 3;
		}
		return true;
	}
}
