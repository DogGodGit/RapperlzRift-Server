using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildHuntingQuestAbandonCommandHandler : InGameCommandHandler<GuildHuntingQuestAbandonCommandBody, GuildHuntingQuestAbandonResponseBody>
{
	public const short kResult_NotExsitQuest = 101;

	private HeroGuildHuntingQuest m_heroGuildHuntingQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_heroGuildHuntingQuest = m_myHero.guildHuntingQuest;
		if (m_heroGuildHuntingQuest == null)
		{
			throw new CommandHandleException(101, "영웅길드헌팅퀘스트가 존재하지 않습니다.");
		}
		m_myHero.guildHuntingQuest = null;
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildHuntingQuest_Status(m_heroGuildHuntingQuest.id, 2, m_currentTime));
		dbWork.Schedule();
	}
}
