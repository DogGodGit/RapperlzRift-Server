using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class BiographyQuestDungeonAbandonCommandHandler : InGameCommandHandler<BiographyQuestDungeonAbandonCommandBody, BiographyQuestDungeonAbandonResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	private BiographyQuestDungeonInstance m_currentPlace;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentPlace = m_myHero.currentPlace as BiographyQuestDungeonInstance;
		if (m_currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_currentPlace.status != 1 && m_currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 실행할 수 없는 명령입니다.");
		}
		m_currentPlace.Finish(5);
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		m_currentPlace.Exit(m_myHero, isLogOut: false, null);
		BiographyQuestDungeonAbandonResponseBody resBody = new BiographyQuestDungeonAbandonResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
