using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RankActiveSkillSelectCommandHandler : InGameCommandHandler<RankActiveSkillSelectCommandBody, RankActiveSkillSelectResponseBody>
{
	public const short kResult_NotExpiredCoolTime = 101;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nTargetSkillId = m_body.targetSkillId;
		HeroRankActiveSkill heroSkill = m_myHero.GetRankActiveSkill(nTargetSkillId);
		if (heroSkill == null)
		{
			throw new CommandHandleException(1, "목표스킬ID가 유효하지 않습니다. nTargetSkillId = " + nTargetSkillId);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.GetRankActiveSkillRemainingCoolTime(currentTime) > 0f)
		{
			throw new CommandHandleException(101, "쿨타임이 만료되지 않았습니다.");
		}
		m_myHero.selectedRankActiveSkill = heroSkill;
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SelectedRankActiveSkill(m_myHero.id, m_myHero.selectedRankActiveSkillId));
		dbWork.Schedule();
	}
}
