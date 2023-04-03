using ClientCommon;

namespace GameServer;

public class ExpDungeonAbandonCommandHandler : InGameCommandHandler<ExpDungeonAbandonCommandBody, ExpDungeonAbandonResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ExpDungeonInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 1 && currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 실행할 수 없는 명령입니다.");
		}
		currentPlace.Finish(4);
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		ExpDungeonAbandonResponseBody resBody = new ExpDungeonAbandonResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
