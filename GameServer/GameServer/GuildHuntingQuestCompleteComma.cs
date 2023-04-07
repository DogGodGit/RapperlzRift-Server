using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildHuntingQuestCompleteCommandHandler : InGameCommandHandler<GuildHuntingQuestCompleteCommandBody, GuildHuntingQuestCompleteResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_UnableInteractionPositionWithCompletionNPC = 102;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private HeroGuildHuntingQuest m_heroGuildHuntingQuest;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서는 사용할 수 없는 명령입니다.");
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_heroGuildHuntingQuest = m_myHero.guildHuntingQuest;
		if (m_heroGuildHuntingQuest == null)
		{
			throw new CommandHandleException(1, "영웅길드헌팅퀘스트가 존재하지 않습니다.");
		}
		if (!m_heroGuildHuntingQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(1, "아직 목표가 완료되지 않았습니다.");
		}
		GuildHuntingQuest guildHuntingQuest = Resource.instance.guildHuntingQuest;
		Npc questNpc = guildHuntingQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "해당 장소에 없는 완료NPC입니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(102, "완료NPC와 상호작용할 수 없는 거리입니다.");
		}
		ItemReward itemReward = guildHuntingQuest.itemReward;
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_NAME_00020", "MAIL_DESC_00020", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.guildHuntingQuest = null;
		SaveToDB();
		SaveToGameLogDB(itemReward);
		GuildHuntingQuestCompleteResponseBody resBody = new GuildHuntingQuestCompleteResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildHuntingQuest_Status(m_heroGuildHuntingQuest.id, 1, m_currentTime));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(ItemReward itemReward)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildHuntingQuestMissionRewardLog(Guid.NewGuid(), m_myGuildMember.guild.id, m_myGuildMember.id, m_heroGuildHuntingQuest.id, itemReward.item.id, itemReward.count, itemReward.owned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
