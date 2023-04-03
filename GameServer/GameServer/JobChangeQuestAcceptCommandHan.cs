using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class JobChangeQuestAcceptCommandHandler : InGameCommandHandler<JobChangeQuestAcceptCommandBody, JobChangeQuestAcceptResponseBody>
{
	public const short kResult_ProgressedQuest = 101;

	public const short kResult_NotEnoughLevel = 102;

	public const short kResult_UnableInteractionPositionWithQuestNPC = 103;

	public const short kResult_NoGuildMember = 104;

	private HeroJobChangeQuest m_newHeroJobChangeQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nQuestNo = m_body.questNo;
		int nDifficulty = m_body.difficulty;
		if (nQuestNo <= 0)
		{
			throw new CommandHandleException(1, "퀘스트번호가 유효하지 않습니다. nQuestNo = " + nQuestNo);
		}
		if (nDifficulty < 0)
		{
			throw new CommandHandleException(1, "유효하지 않는 난이도입니다. nDifficulty = " + nDifficulty);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.level < Resource.instance.jobChangeRequiredHeroLevel)
		{
			throw new CommandHandleException(102, "레벨이 부족합니다.");
		}
		HeroJobChangeQuest heroJobChangeQuest = m_myHero.jobChangeQuest;
		if (heroJobChangeQuest != null && heroJobChangeQuest.isAccepted)
		{
			throw new CommandHandleException(101, "이미 퀘스트를 진행중입니다.");
		}
		int nTargetQuestNo = 0;
		nTargetQuestNo = ((heroJobChangeQuest == null) ? 1 : ((!heroJobChangeQuest.isFailed) ? (heroJobChangeQuest.quest.no + 1) : heroJobChangeQuest.quest.no));
		if (nTargetQuestNo != nQuestNo)
		{
			throw new CommandHandleException(1, "진행해야될 퀘스트번호가 아닙니다.");
		}
		JobChangeQuest jobChangeQuest = Resource.instance.GetJobChangeQuest(nTargetQuestNo);
		if (jobChangeQuest == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 퀘스트입니다.");
		}
		if (jobChangeQuest.type == JobChangeQuestType.ExclusiveMonsterHunt && nDifficulty <= 0)
		{
			throw new CommandHandleException(1, "난이도를 선택해야합니다. nDifficulty = " + nDifficulty);
		}
		Npc questNpc = jobChangeQuest.questNpc;
		if (questNpc == null)
		{
			throw new CommandHandleException(1, "퀘스트NPC가 존재하지 않습니다.");
		}
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 NPC입니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "해당 NPC와 상호작용할 수 없는 거리입니다.");
		}
		if (jobChangeQuest.type == JobChangeQuestType.ExclusiveMonsterHunt)
		{
			JobChangeQuestDifficulty jobChangeQuestDifficulty = jobChangeQuest.GetDifficulty(nDifficulty);
			if (jobChangeQuestDifficulty == null)
			{
				throw new CommandHandleException(1, "존재하지 않는 난이도입니다. nDifficulty = " + nDifficulty);
			}
			if (jobChangeQuestDifficulty.isTargetPlaceGuildTerrtory && m_myHero.guildMember == null)
			{
				throw new CommandHandleException(104, "길드에 가입하지 않았습니다.");
			}
		}
		m_newHeroJobChangeQuest = new HeroJobChangeQuest(m_myHero, jobChangeQuest, nDifficulty, m_currentTime);
		m_myHero.jobChangeQuest = m_newHeroJobChangeQuest;
		if (jobChangeQuest.limitTime > 0)
		{
			m_newHeroJobChangeQuest.StartLimitTimer(jobChangeQuest.limitTime * 1000);
		}
		SaveToDB();
		JobChangeQuestAcceptResponseBody resBody = new JobChangeQuestAcceptResponseBody();
		resBody.quest = m_myHero.jobChangeQuest.ToPDHeroJobChangeQuest(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroJobChangeQuest(m_newHeroJobChangeQuest.instanceId, m_newHeroJobChangeQuest.quest.no, m_newHeroJobChangeQuest.hero.id, m_newHeroJobChangeQuest.difficulty, m_currentTime));
		dbWork.Schedule();
	}
}
