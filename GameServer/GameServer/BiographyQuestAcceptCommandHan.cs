using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class BiographyQuestAcceptCommandHandler : InGameCommandHandler<BiographyQuestAcceptCommandBody, BiographyQuestAcceptResponseBody>
{
	public const short kResult_NotCompletedQuest = 101;

	public const short kResult_LastQuest = 102;

	public const short kResult_UnableInteractionPositionWithStartNPC = 103;

	private HeroBiographyQuest m_newBiographyQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nBiographyId = m_body.biographyId;
		int nQuestNo = m_body.questNo;
		if (nBiographyId <= 0)
		{
			throw new CommandHandleException(1, "전기ID가 유효하지 않습니다. nBiographyId = " + nBiographyId);
		}
		if (nQuestNo <= 0)
		{
			throw new CommandHandleException(1, "퀘스트번호가 유효하지 않습니다. nQuestNo = " + nQuestNo);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		HeroBiography heroBiography = m_myHero.GetBiography(nBiographyId);
		if (heroBiography == null)
		{
			throw new CommandHandleException(1, "영웅전기가 존재하지 않습니다. nBiographyId = " + nBiographyId);
		}
		HeroBiographyQuest heroBiographyQuest = heroBiography.quest;
		int nNextQuestNo = 1;
		if (heroBiographyQuest != null)
		{
			if (!heroBiographyQuest.completed)
			{
				throw new CommandHandleException(101, "현재 영웅전기퀘스트가 완료되지 않았습니다.");
			}
			if (heroBiographyQuest.isLastQuest)
			{
				throw new CommandHandleException(102, "현재 퀘스트가 마지막 퀘스트입니다.");
			}
			nNextQuestNo = heroBiographyQuest.quest.no + 1;
		}
		if (nNextQuestNo != nQuestNo)
		{
			throw new CommandHandleException(1, "진행해야될 퀘스트번호가 아닙니다. nQuestNo = " + nQuestNo);
		}
		BiographyQuest biographyQuest = heroBiography.biography.GetQuest(nQuestNo);
		Npc startNpc = biographyQuest.startNpc;
		if (startNpc != null)
		{
			if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
			{
				throw new CommandHandleException(1, "현재 장소에 없는 NPC입니다.");
			}
			if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
			{
				throw new CommandHandleException(103, "시작 NPC와 상호작용할 수 있는 위치가 아닙니다.");
			}
		}
		m_newBiographyQuest = new HeroBiographyQuest(heroBiography, biographyQuest);
		heroBiography.SetQuest(m_newBiographyQuest);
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroBiographyQuest(m_newBiographyQuest.biography.hero.id, m_newBiographyQuest.biography.biography.id, m_newBiographyQuest.quest.no, m_currentTime));
		dbWork.Schedule();
	}
}
