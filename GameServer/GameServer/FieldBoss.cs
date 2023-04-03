using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldBoss
{
	private FieldBossEvent m_fieldBossEvent;

	private int m_nId;

	private MonsterArrange m_monsterArrange;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private ItemReward m_itemReward;

	public FieldBossEvent fieldBossEvent => m_fieldBossEvent;

	public int id => m_nId;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Continent continent => m_continent;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public ItemReward itemReward => m_itemReward;

	public FieldBoss(FieldBossEvent fieldBossEvent)
	{
		m_fieldBossEvent = fieldBossEvent;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nId = Convert.ToInt32(dr["fieldBossId"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = res.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
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
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = res.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
