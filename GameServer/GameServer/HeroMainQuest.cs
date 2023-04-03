using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroMainQuest
{
	private Hero m_hero;

	private MainQuest m_mainQuest;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private bool m_bCompleted;

	private bool m_bIsCartRiding;

	private int m_nCartContinentId;

	private Vector3 m_cartPosition = Vector3.zero;

	private float m_fCartRotationY;

	private MainQuestCartInstance m_cartInst;

	public Hero hero => m_hero;

	public MainQuest mainQuest => m_mainQuest;

	public int progressCount
	{
		get
		{
			return m_nProgressCount;
		}
		set
		{
			m_nProgressCount = value;
		}
	}

	public bool completed
	{
		get
		{
			return m_bCompleted;
		}
		set
		{
			m_bCompleted = value;
		}
	}

	public bool isCartRiding => m_bIsCartRiding;

	public int cartContinentId => m_nCartContinentId;

	public Vector3 cartPosition => m_cartPosition;

	public float cartRotationY => m_fCartRotationY;

	public MainQuestCartInstance cartInst
	{
		get
		{
			return m_cartInst;
		}
		set
		{
			m_cartInst = value;
		}
	}

	public bool isObjectiveCompleted => m_nProgressCount >= m_mainQuest.targetCount;

	public HeroMainQuest(Hero hero)
		: this(hero, null)
	{
	}

	public HeroMainQuest(Hero hero, MainQuest mainQuest)
	{
		m_hero = hero;
		m_mainQuest = mainQuest;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMainQuestNo = Convert.ToInt32(dr["mainQuestNo"]);
		m_mainQuest = Resource.instance.GetMainQuest(nMainQuestNo);
		if (m_mainQuest == null)
		{
			throw new Exception("메인퀘스트가 존재하지 않습니다. nMainQuestNo = " + nMainQuestNo);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
		m_bIsCartRiding = Convert.ToBoolean(dr["isCartRiding"]);
		m_nCartContinentId = Convert.ToInt32(dr["cartContinentId"]);
		m_cartPosition.x = Convert.ToSingle(dr["cartXPosition"]);
		m_cartPosition.y = Convert.ToSingle(dr["cartYPosition"]);
		m_cartPosition.z = Convert.ToSingle(dr["cartZPosition"]);
		m_fCartRotationY = Convert.ToSingle(dr["cartYRotation"]);
	}

	public void SetProgressCount(int nCount)
	{
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (m_nProgressCount != nCount)
		{
			m_nProgressCount = nCount;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainQuest_ProgressCount(m_hero.id, m_mainQuest.no, m_nProgressCount));
			dbWork.Schedule();
			ServerEvent.SendMainQuestUpdated(m_hero.account.peer, m_mainQuest.no, m_nProgressCount);
		}
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainQuest_ProgressCount(m_hero.id, m_mainQuest.no, m_nProgressCount));
		dbWork.Schedule();
		ServerEvent.SendMainQuestUpdated(m_hero.account.peer, m_mainQuest.no, m_nProgressCount);
	}

	public void IncreaseProgressCountByMonsterQuest(long lnMonsterInstanceId)
	{
		if (m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			IncreaseProgressCount();
		}
	}

	public void RefreshCartInfo()
	{
		m_bIsCartRiding = false;
		m_nCartContinentId = 0;
		m_cartPosition = Vector3.zero;
		m_fCartRotationY = 0f;
		if (m_cartInst != null)
		{
			m_bIsCartRiding = m_cartInst.isRiding;
			NationContinentInstance currentPlace = (NationContinentInstance)m_cartInst.currentPlace;
			if (currentPlace != null)
			{
				m_nCartContinentId = currentPlace.continent.id;
				m_cartPosition = m_cartInst.position;
				m_fCartRotationY = m_cartInst.rotationY;
			}
		}
	}

	public PDHeroMainQuest ToPDHeroMainQuest()
	{
		PDHeroMainQuest inst = new PDHeroMainQuest();
		inst.no = m_mainQuest.no;
		inst.progressCount = m_nProgressCount;
		inst.completed = m_bCompleted;
		inst.cartInstanceId = ((m_cartInst != null) ? m_cartInst.instanceId : 0);
		inst.cartContinentId = m_nCartContinentId;
		inst.cartPosition = m_cartPosition;
		inst.cartRotationY = m_fCartRotationY;
		return inst;
	}
}
