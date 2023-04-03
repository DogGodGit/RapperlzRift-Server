using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BiographyQuestNpcConversationCompleteCommandHandler : InGameCommandHandler<BiographyQuestNpcConversationCompleteCommandBody, BiographyQuestNpcConversationCompleteResponseBody>
{
	private List<HeroBiography> m_targetHeroBiographies = new List<HeroBiography>();

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		foreach (HeroBiography heroBiography in m_myHero.biographies.Values)
		{
			if (heroBiography.completed)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography.quest;
			if (heroBiographyQuest == null || heroBiographyQuest.isObjectiveCompleted)
			{
				continue;
			}
			BiographyQuest biographyQuest = heroBiographyQuest.quest;
			if (biographyQuest.type == BiographyQuestType.NpcConversation)
			{
				Npc targetNpc = biographyQuest.targetNpc;
				if (currentPlace.IsSame(targetNpc.continent.id, m_myHero.nationId) && targetNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
				{
					heroBiographyQuest.IncreaseProgressCount();
					m_targetHeroBiographies.Add(heroBiography);
				}
			}
		}
		SaveToDB();
		BiographyQuestNpcConversationCompleteResponseBody resBody = new BiographyQuestNpcConversationCompleteResponseBody();
		resBody.progressCounts = HeroBiography.ToPDHeroBiographyQuestProgressCounts(m_targetHeroBiographies).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (HeroBiography heroBiography in m_targetHeroBiographies)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiography_ProgressCount(heroBiography.hero.id, heroBiography.biography.id, heroBiography.quest.quest.no, heroBiography.quest.progressCount));
		}
		dbWork.Schedule();
	}
}
