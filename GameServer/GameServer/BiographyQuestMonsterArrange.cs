using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BiographyQuestMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private int m_nKey;

	private BiographyQuestDungeonWave m_wave;

	private BiographyQuestMonsterArrangeType m_type = BiographyQuestMonsterArrangeType.NormalMonster;

	private MonsterArrange m_monsterArrange;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nRotationYType;

	private float m_fRotationY;

	private int m_nMonsterCount;

	public int key => m_nKey;

	public BiographyQuestDungeonWave wave => m_wave;

	public BiographyQuestMonsterArrangeType type => m_type;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int rotationYType => m_nRotationYType;

	public float rotationY => m_fRotationY;

	public int monsterCount => m_nMonsterCount;

	public BiographyQuestMonsterArrange(BiographyQuestDungeonWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nKey = Convert.ToInt32(dr["arrangeKey"]);
		if (m_nKey <= 0)
		{
			SFLogUtil.Warn(GetType(), "배치키가 유효하지 않습니다. m_nKey = " + m_nKey);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(BiographyQuestMonsterArrangeType), nType))
		{
			SFLogUtil.Warn(GetType(), "전기퀘스트몬스터배치타입이 유효하지 않습니다. m_nKey = " + m_nKey + ", nType = " + nType);
		}
		m_type = (BiographyQuestMonsterArrangeType)nType;
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
		if (m_monsterArrange == null)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치가 유효하지 않습니다. m_nKey = " + m_nKey + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반경이 유효하지 않습니다. m_nKey = " + m_nKey + ", m_fRadius = " + m_fRadius);
		}
		m_nRotationYType = Convert.ToInt32(dr["yRotationType"]);
		m_fRotationY = Convert.ToSingle(dr["yRotation"]);
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. m_nKey = " + m_nKey + ", m_nMonsterCount = " + m_nMonsterCount);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nRotationYType != 1)
		{
			return SFRandom.NextFloat(0f, m_fRotationY);
		}
		return m_fRotationY;
	}
}
