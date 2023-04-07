using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class MainQuestDungeonExitCommandHandler : InGameCommandHandler<MainQuestDungeonExitCommandBody, MainQuestDungeonExitResponseBody>
{
	public const short kResult_NotStatusFinished = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is MainQuestDungeonInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!currentPlace.isFinished)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		if (currentPlace.status == 3 && currentPlace.mainQuestDungeon.completionExitPositionEnabled)
		{
			m_myHero.SetPreviousPositionAndRotation(currentPlace.mainQuestDungeon.completionExitPosition, currentPlace.mainQuestDungeon.completionExitYRotation);
		}
		MainQuestDungeonExitResponseBody resBody = new MainQuestDungeonExitResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
