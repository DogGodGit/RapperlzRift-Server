using System;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroJobChangeQuest
{
	private Guid m_instanceId = Guid.Empty;

	private JobChangeQuest m_quest;

	private Hero m_hero;

	private int m_nProgressCount;

	private int m_nDifficulty;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private HeroJobChangeQuestStatus m_status;

	private JobChangeQuestMonsterInstance m_monsterInst;

	private Timer m_limitTimer;

	public Guid instanceId => m_instanceId;

	public JobChangeQuest quest => m_quest;

	public Hero hero => m_hero;

	public int progressCount => m_nProgressCount;

	public bool isObjectiveCompleted => m_nProgressCount >= m_quest.targetCount;

	public int difficulty => m_nDifficulty;

	public DateTimeOffset regTime => m_regTime;

	public HeroJobChangeQuestStatus status => m_status;

	public bool isAccepted => m_status == HeroJobChangeQuestStatus.Acception;

	public bool isCompleted => m_status == HeroJobChangeQuestStatus.Completion;

	public bool isFailed => m_status == HeroJobChangeQuestStatus.Fail;

	public JobChangeQuestMonsterInstance monsterInst
	{
		get
		{
			return m_monsterInst;
		}
		set
		{
			m_monsterInst = value;
		}
	}

	public Timer limitTimer => m_limitTimer;

	public HeroJobChangeQuest(Hero hero)
		: this(hero, null, 0, DateTimeOffset.MinValue)
	{
	}

	public HeroJobChangeQuest(Hero hero, JobChangeQuest quest, int nDifficulty, DateTimeOffset regTime)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_instanceId = Guid.NewGuid();
		m_hero = hero;
		m_quest = quest;
		m_nDifficulty = nDifficulty;
		m_regTime = regTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_instanceId = (Guid)dr["questInstanceId"];
		int nQuestNo = Convert.ToInt32(dr["questNo"]);
		m_quest = Resource.instance.GetJobChangeQuest(nQuestNo);
		if (m_quest == null)
		{
			throw new Exception("전직퀘스트가 존재하지 않습니다. nQuestNo = " + nQuestNo);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
		int nStatus = Convert.ToInt32(dr["status"]);
		if (Enum.IsDefined(typeof(HeroJobChangeQuestStatus), nStatus))
		{
			m_status = (HeroJobChangeQuestStatus)nStatus;
		}
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		if (m_quest.limitTime <= 0)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_regTime).TotalSeconds;
		return Math.Max((float)m_quest.limitTime - fElapsedTime, 0f);
	}

	public void StartLimitTimer(int nDuration)
	{
		if (nDuration > 0)
		{
			m_limitTimer = new Timer(OnLimitTimerTick);
			m_limitTimer.Change(nDuration, -1);
		}
	}

	private void OnLimitTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLimitTimerTickTick));
	}

	private void ProcessLimitTimerTickTick()
	{
		if (m_limitTimer != null)
		{
			DisposeLimitTimer();
			Fail(bSendEvent: true, DateTimeUtil.currentTime);
		}
	}

	public void DisposeLimitTimer()
	{
		if (m_limitTimer != null)
		{
			m_limitTimer.Dispose();
			m_limitTimer = null;
		}
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		ServerEvent.SendJobChangeQuestProgressCountUpdated(m_hero.account.peer, m_instanceId, m_nProgressCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroJobChangeQuest_ProgressCount(m_instanceId, m_nProgressCount));
		dbWork.Schedule();
	}

	public void Complete()
	{
		m_status = HeroJobChangeQuestStatus.Completion;
		if (m_monsterInst != null)
		{
			m_monsterInst = null;
		}
	}

	public void RemoveExclusiveMonster()
	{
		if (m_monsterInst == null)
		{
			return;
		}
		Place monsterCurrentPlace = monsterInst.currentPlace;
		lock (monsterCurrentPlace.syncObject)
		{
			monsterCurrentPlace.RemoveMonster(monsterInst, bSendEvent: true);
			m_monsterInst = null;
		}
	}

	public void Fail(bool bSendEvent, DateTimeOffset currentTime)
	{
		m_status = HeroJobChangeQuestStatus.Fail;
		if (m_monsterInst != null)
		{
			m_monsterInst = null;
		}
		if (bSendEvent)
		{
			ServerEvent.SendJobChangeQuestFailed(m_hero.account.peer, m_instanceId);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroJobChangeQuest_Status(m_instanceId, 2, currentTime));
		dbWork.Schedule();
	}

	public PDHeroJobChangeQuest ToPDHeroJobChangeQuest(DateTimeOffset time)
	{
		PDHeroJobChangeQuest inst = new PDHeroJobChangeQuest();
		inst.instanceId = (Guid)m_instanceId;
		inst.questNo = m_quest.no;
		inst.progressCount = m_nProgressCount;
		inst.difficulty = m_nDifficulty;
		inst.remainingTime = GetRemainingTime(time);
		inst.status = (int)m_status;
		if (m_monsterInst != null)
		{
			lock (m_monsterInst.currentPlace.syncObject)
			{
				inst.monsterInstanceId = m_monsterInst.instanceId;
				inst.monsterPosition = m_monsterInst.position;
				return inst;
			}
		}
		return inst;
	}
}
