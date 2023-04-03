using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimSummonMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private RuinsReclaimMonsterArrange m_arrange;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private Vector3 m_position = Vector3.zero;

	private int m_nYRotationType;

	private float m_fYRotation;

	private float m_fBossMonsterHpRecoveryFactor;

	public RuinsReclaimMonsterArrange arrange => m_arrange;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Vector3 position => m_position;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public float bossMonsterHpRecoveryFactor => m_fBossMonsterHpRecoveryFactor;

	public RuinsReclaimSummonMonsterArrange(RuinsReclaimMonsterArrange arrange)
	{
		m_arrange = arrange;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["summonNo"]);
		long lnMonsterArrangeId = Convert.ToInt32(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. arrangeKey = " + m_arrange.key + ", m_nNo =" + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. arrangeKey = " + m_arrange.key + ", m_nNo =" + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. arrangeKey = " + m_arrange.key + ", m_nNo =" + m_nNo + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fBossMonsterHpRecoveryFactor = Convert.ToSingle(dr["bossMonsterHpRecoveryFactor"]);
		if (m_fBossMonsterHpRecoveryFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터HP회복계수가 유효하지 않습니다. arrangeKey = " + m_arrange.key + ", m_nNo =" + m_nNo + ", m_fBossMonsterHpRecoveryFactor = " + m_fBossMonsterHpRecoveryFactor);
		}
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
