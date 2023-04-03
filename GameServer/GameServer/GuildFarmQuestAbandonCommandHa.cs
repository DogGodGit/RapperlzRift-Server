using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildFarmQuestAbandonCommandHandler : InGameCommandHandler<GuildFarmQuestAbandonCommandBody, GuildFarmQuestAbandonResponseBody>
{
	public const short kResult_PerformingQuestNotExist = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HeroGuildFarmQuest m_heroQuest;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_heroQuest = m_myHero.guildFarmQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(101, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		m_myHero.RemoveGuildFarmQuest();
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildFarmQuest_Status(m_heroQuest.id, 2, m_currentTime));
		dbWork.Schedule();
	}
}
