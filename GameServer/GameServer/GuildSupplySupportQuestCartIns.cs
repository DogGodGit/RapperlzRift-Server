using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildSupplySupportQuestCartInstance : CartInstance
{
	private GuildSupplySupportQuestPlay m_quest;

	public override CartInstanceType cartInstanceType => CartInstanceType.GuildSupplySupportQuest;

	public override bool abnormalStateHitEnabled => true;

	public GuildSupplySupportQuestPlay quest => m_quest;

	public void Init(GuildSupplySupportQuestPlay quest, Hero hero, DateTimeOffset time, bool bFirstCreation)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		InitCartInstance(quest.cart, hero, time);
		m_quest = quest;
		m_quest.cartInst = this;
		if (!bFirstCreation)
		{
			m_nHP = quest.cartHp;
		}
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

	protected override void OnDead()
	{
		base.OnDead();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_owner.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuildSupplySupportQuest_Cart(m_quest));
		dbWork.Schedule();
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		m_quest.Fail(time, bSendFailEvent: true);
	}

	protected override PDCartInstance CreatePDCartInstance()
	{
		return new PDGuildSupplySupportQuestCartInstance();
	}
}
