using System;
using ClientCommon;

namespace GameServer;

public class SceneryQuestStartCommandHandler : InGameCommandHandler<SceneryQuestStartCommandBody, SceneryQuestStartResponseBody>
{
	public const short kResult_AlreadyCompleted = 101;

	public const short kResult_AlreadyStarted = 102;

	public const short kResult_UnableQuestStartPositionWithQuestArea = 103;

	public const short kResult_NotCompletedRequiredMainQuest = 104;

	protected override void HandleInGameCommand()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nQuestId = m_body.questId;
		Resource res = Resource.instance;
		SceneryQuest quest = res.GetSceneryQuest(nQuestId);
		if (quest == null)
		{
			throw new CommandHandleException(1, "대상 퀘스트ID가 유효하지 않습니다. nQuestId = " + nQuestId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에서는 사용할 수 없는 명령입니다.");
		}
		if (currentPlace is NationContinentInstance && currentPlace.nationId != m_myHero.nationId)
		{
			throw new CommandHandleException(1, "퀘스트 시작을 위한 국가에 있지 않습니다.");
		}
		if (currentPlace.continent.id != quest.continentId)
		{
			throw new CommandHandleException(1, "퀘스트 시작을 위한 대륙에 있지 않습니다.");
		}
		if (!m_myHero.IsMainQuestCompleted(res.sceneryQuestRequiredMainQuestNo))
		{
			throw new CommandHandleException(104, "필요한 메인퀘스트를 완료하지 않았습니다.");
		}
		if (m_myHero.ContainsSceneryQuestCompletion(nQuestId))
		{
			throw new CommandHandleException(101, "대상 퀘스트를 이미 완료했습니다. nQuestId = " + nQuestId);
		}
		if (m_myHero.GetSceneryQuest(nQuestId) != null)
		{
			throw new CommandHandleException(102, "대상 퀘스트를 이미 진행중입니다. nQuestId = " + nQuestId);
		}
		if (!quest.IsSceneryQuestAreaPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "퀘스트를 시작할 수 없는 위치입니다.");
		}
		HeroSceneryQuest heroQuest = new HeroSceneryQuest(m_myHero);
		heroQuest.Init(quest, currentTime);
		m_myHero.AddSceneryQuest(heroQuest);
		SceneryQuestStartResponseBody resBody = new SceneryQuestStartResponseBody();
		resBody.remainingTime = heroQuest.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
