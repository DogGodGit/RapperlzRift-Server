using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroSceneryQuest
{
	private Hero m_hero;

	private SceneryQuest m_quest;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	public SceneryQuest quest => m_quest;

	public int questId => m_quest.id;

	public DateTimeOffset startTime => m_startTime;

	public HeroSceneryQuest(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(SceneryQuest quest, DateTimeOffset time)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
		m_startTime = time;
	}

	public void OnUpdate(DateTimeOffset time)
	{
		if (!IsSceneryQuestArea())
		{
			CancelQuest();
		}
		else if (!(GetRemainingTime(time) > 0f))
		{
			CompleteQuest();
		}
	}

	private bool IsSceneryQuestArea()
	{
		if (!(m_hero.currentPlace is ContinentInstance currentPlace))
		{
			return false;
		}
		if (currentPlace.nationId != m_hero.nationId)
		{
			return false;
		}
		if (currentPlace.continent.id != m_quest.continentId)
		{
			return false;
		}
		if (!m_quest.IsSceneryQuestAreaPosition(m_hero.position, m_hero.radius))
		{
			return false;
		}
		return true;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		TimeSpan questTime = time - m_startTime;
		return Math.Max((float)((double)m_quest.waitingTime - questTime.TotalSeconds), 0f);
	}

	private void CancelQuest()
	{
		m_hero.RemoveSceneryQuest(questId);
		ServerEvent.SendSceneryQuestCanceled(m_hero.account.peer, questId);
	}

	private void CompleteQuest()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		ItemReward itemReward = m_quest.itemReward;
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Mail mail = null;
		PDItemBooty booty = null;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainingCount = m_hero.AddItem(item, bOwned, nCount, changedInventorySlots);
			if (nRewardItemRemainingCount > 0)
			{
				mail = Mail.Create("MAIL_NAME_00017", "MAIL_DESC_00017", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainingCount, bOwned));
				m_hero.AddMail(mail, bSendEvent: true);
			}
			booty = new PDItemBooty();
			booty.id = item.id;
			booty.count = nCount;
			booty.owned = bOwned;
		}
		m_hero.AddSceneryQuestCompletion(questId);
		m_hero.RemoveSceneryQuest(questId);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroSceneryQuest(m_hero.id, questId));
		dbWork.Schedule();
		ServerEvent.SendSceneryQuestCompleted(m_hero.account.peer, questId, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
	}
}
