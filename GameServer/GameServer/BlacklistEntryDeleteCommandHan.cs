using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BlacklistEntryDeleteCommandHandler : InGameCommandHandler<BlacklistEntryDeleteCommandBody, BlacklistEntryDeleteResponseBody>
{
	public const short kResult_NotExistInBlacklist = 101;

	private Guid m_targetHeroId = Guid.Empty;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_targetHeroId = (Guid)m_body.targetHeroId;
		if (m_targetHeroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
		}
		if (!m_myHero.IsBlacklistEntry(m_targetHeroId))
		{
			throw new CommandHandleException(101, "대상영웅은 블랙리스트에 존재하지 않습니다. m_targetHeroId = " + m_targetHeroId);
		}
		m_myHero.RemoveBlacklistEntry(m_targetHeroId);
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_DeleteBlacklistEntry(m_myHero.id, m_targetHeroId));
		dbWork.Schedule();
	}
}
