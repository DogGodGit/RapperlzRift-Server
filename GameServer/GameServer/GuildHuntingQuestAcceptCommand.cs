using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildHuntingQuestAcceptCommandHandler : InGameCommandHandler<GuildHuntingQuestAcceptCommandBody, GuildHuntingQuestAcceptResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_UnableInteractionPositionWithStartNPC = 102;

	public const short kResult_DailyQuestCountOverflowed = 103;

	public const short kResult_AlreadyConsigned = 104;

	private Guild m_myGuild;

	private HeroGuildHuntingQuest m_addedGuildHuntingQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (m_myHero.guildHuntingQuest != null)
		{
			throw new CommandHandleException(1, "아직 길드헌팅퀘스트가 진행중입니다.");
		}
		m_myGuild = myGuildMember.guild;
		GuildHuntingQuest guildHuntingQuest = Resource.instance.guildHuntingQuest;
		Npc questNpc = guildHuntingQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 NPC입니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(102, "시작 NPC와 상호작용할 수 없는 위치입니다.");
		}
		if (m_myHero.GetRemainingTaskConsignmentStartCount(3, m_currentTime.Date) <= 0)
		{
			throw new CommandHandleException(104, "이미 위탁을 한 할일입니다.");
		}
		GuildHuntingQuestObjectivePool pool = guildHuntingQuest.GetObjectivePoolOfHeroLevel(m_myHero.level);
		if (pool == null)
		{
			throw new CommandHandleException(1, "목표풀이 존재하지 않습니다.");
		}
		m_myHero.RefreshDailyGuildHuntingQuestCount(m_currentTime.Date);
		DateValuePair<int> dailyGuildHuntingQuestCount = m_myHero.dailyGuildHuntingQuestCount;
		if (dailyGuildHuntingQuestCount.value >= guildHuntingQuest.limitCount)
		{
			throw new CommandHandleException(103, "일일횟수가 최대횟수를 넘어갑니다.");
		}
		GuildHuntingQuestObjective objective = pool.SelectObjective();
		m_addedGuildHuntingQuest = new HeroGuildHuntingQuest(myGuildMember, objective);
		m_myHero.guildHuntingQuest = m_addedGuildHuntingQuest;
		dailyGuildHuntingQuestCount.value++;
		SaveToDB();
		GuildHuntingQuestAcceptResponseBody resBody = new GuildHuntingQuestAcceptResponseBody();
		resBody.date = (DateTime)dailyGuildHuntingQuestCount.date;
		resBody.dailyGuildHuntingQuestStartCount = dailyGuildHuntingQuestCount.value;
		resBody.guildHuntingQuest = m_addedGuildHuntingQuest.ToPDHeroGuildHuntingQuest();
		SendResponseOK(resBody);
		if (dailyGuildHuntingQuestCount.value >= guildHuntingQuest.limitCount)
		{
			m_myGuild.CompleteGuildDailyObjective(m_currentTime.Date, 2, myGuildMember);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildHuntingQuest(m_addedGuildHuntingQuest.id, m_addedGuildHuntingQuest.guildId, m_addedGuildHuntingQuest.hero.id, m_addedGuildHuntingQuest.objective.id, m_currentTime, m_currentTime));
		dbWork.Schedule();
	}
}
