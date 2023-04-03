using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainQuestCartInstance : CartInstance
{
	private HeroMainQuest m_quest;

	public override bool hitEnabled => false;

	public override bool abnormalStateHitEnabled => false;

	public override bool abnormalStateDamageEnabled => false;

	public override CartInstanceType cartInstanceType => CartInstanceType.MainQuest;

	public HeroMainQuest quest => m_quest;

	public void Init(HeroMainQuest quest, DateTimeOffset currentTime)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		InitCartInstance(quest.mainQuest.cart, quest.hero, currentTime);
		m_quest = quest;
		m_quest.cartInst = this;
		Cache.instance.AddCartInstance(this);
	}

	protected override void OnGetOn()
	{
		base.OnGetOn();
		m_quest.RefreshCartInfo();
	}

	protected override void OnGetOff()
	{
		base.OnGetOff();
		m_quest.RefreshCartInfo();
	}

	protected override void OnUpdateInternal()
	{
		base.OnUpdateInternal();
		AddWork(new SFAction(OnUpdate_ProcessMainQuestObjective), bGlobalLockRequired: true);
	}

	private void OnUpdate_ProcessMainQuestObjective()
	{
		try
		{
			lock (m_owner.syncObject)
			{
				ProcessMainQuestObjective();
			}
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void ProcessMainQuestObjective()
	{
		if (m_owner.isInitEntranceCopmleted)
		{
			MainQuest mainQuest = m_quest.mainQuest;
			int nProgressCount = 0;
			if (m_currentPlace is ContinentInstance currentPlace && currentPlace.IsSame(mainQuest.targetContinent.id, m_owner.nationId) && mainQuest.TargetAreaContains(m_position))
			{
				nProgressCount = 1;
			}
			m_quest.SetProgressCount(nProgressCount);
		}
	}

	protected override PDCartInstance CreatePDCartInstance()
	{
		return new PDMainQuestCartInstance();
	}
}
