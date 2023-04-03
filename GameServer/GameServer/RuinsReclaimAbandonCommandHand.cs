using ClientCommon;

namespace GameServer;

public class RuinsReclaimAbandonCommandHandler : InGameCommandHandler<RuinsReclaimAbandonCommandBody, RuinsReclaimAbandonResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is RuinsReclaimInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 2 && currentPlace.status != 1)
		{
			throw new CommandHandleException(101, "현재 상태에서 상용할 수 없는 명령입니다.");
		}
		currentPlace.Disqualification(m_myHero);
		m_myHero.CancelRuinsReclaimMonsterTransformation();
		m_myHero.StopRuinsReclaimDebuffEffect(bSendEvent: false);
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		RuinsReclaimAbandonResponseBody resBody = new RuinsReclaimAbandonResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
