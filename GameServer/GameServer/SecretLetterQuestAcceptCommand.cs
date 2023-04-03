using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SecretLetterQuestAcceptCommandHandler : InGameCommandHandler<SecretLetterQuestAcceptCommandBody, SecretLetterQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.secretLetterQuest != null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재합니다.");
		}
		SecretLetterQuest quest = Resource.instance.secretLetterQuest;
		if (m_myHero.level < quest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 부족합니다. heroLevel = " + m_myHero.level);
		}
		m_myHero.RefreshDailySecretLetterQuestStartCount(m_currentDate);
		DateValuePair<int> startCount = m_myHero.dailySecretLetterQuestStartCount;
		if (startCount.value >= quest.limitCount)
		{
			throw new CommandHandleException(1, "금일 시작횟수를 모두 사용했습니다.");
		}
		Npc questNpc = quest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		HeroSecretLetterQuest heroQuest = new HeroSecretLetterQuest(m_myHero, m_myHero.nationInst.secretLetterQuestTargetNationId);
		m_myHero.secretLetterQuest = heroQuest;
		startCount.value++;
		SaveToDB();
		SecretLetterQuestAcceptResponseBody resBody = new SecretLetterQuestAcceptResponseBody();
		resBody.quest = heroQuest.ToPDHeroSecretLetterQuest();
		resBody.date = (DateTime)startCount.date;
		resBody.dailyStartCount = startCount.value;
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(5, m_currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(13);
		m_myHero.ProcessMainQuestForContent(8);
		m_myHero.ProcessSubQuestForContent(8);
	}

	private void SaveToDB()
	{
		HeroSecretLetterQuest heroQuest = m_myHero.secretLetterQuest;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddSecretLetterQuest(heroQuest.id, m_myHero.id, heroQuest.targetNationId, m_currentTime));
		dbWork.Schedule();
	}
}
