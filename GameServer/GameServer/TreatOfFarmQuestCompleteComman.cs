using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestCompleteCommandHandler : InGameCommandHandler<TreatOfFarmQuestCompleteCommandBody, TreatOfFarmQuestCompleteResponseBody>
{
	private Mail m_mail;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		_ = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		HeroTreatOfFarmQuest heroTreatOfFarmQuest = m_myHero.treatOfFarmQuest;
		if (heroTreatOfFarmQuest == null)
		{
			throw new CommandHandleException(1, "농장의위협퀘스트가 존재하지 않습니다.");
		}
		if (!heroTreatOfFarmQuest.objectiveCompleted)
		{
			throw new CommandHandleException(1, "농장의위협 퀘스트 목표를 완료하지 못했습니다.");
		}
		if (heroTreatOfFarmQuest.completed)
		{
			throw new CommandHandleException(1, "농장의위협 퀘스트를 이미 완료했습니다.");
		}
		TreatOfFarmQuest treatOfFarmQuest = Resource.instance.treatOfFarmQuest;
		Npc questNpc = treatOfFarmQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		TreatOfFarmQuestReward reward = treatOfFarmQuest.GetReward(m_myHero.level);
		ItemReward itemReward = reward.questCompletionItemReward;
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_NAME_00019", "MAIL_DESC_00019", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		heroTreatOfFarmQuest.completed = true;
		SaveToDB(heroTreatOfFarmQuest);
		TreatOfFarmQuestCompleteResponseBody resBody = new TreatOfFarmQuestCompleteResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroTreatOfFarmQuest heroTreatOfFarmQuest)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateTreatOfFarmQuest_Complete(heroTreatOfFarmQuest.id, m_currentTime));
		dbWork.Schedule();
	}
}
