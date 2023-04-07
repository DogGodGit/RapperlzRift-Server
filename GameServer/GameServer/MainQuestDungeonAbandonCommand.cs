using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class MainQuestDungeonAbandonCommandHandler : InGameCommandHandler<MainQuestDungeonAbandonCommandBody, MainQuestDungeonAbandonResponseBody>
{
	public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is MainQuestDungeonInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 1 && currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 실행할 수 없는 명령입니다.");
		}
		currentPlace.Finish(5);
		_ = DateTimeUtil.currentTime;
		if (m_myHero.isDead)
		{
			m_myHero.Revive(bSendEvent: false);
		}
		else
		{
			m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		int nPreviousNationId = m_myHero.previousNationId;
		if (nPreviousNationId != 0 && nPreviousNationId != m_myHero.nationId)
		{
			NationInstance nationInst = Cache.instance.GetNationInstance(nPreviousNationId);
			NationWarInstance nationWarInst = nationInst.nationWarInst;
			if (nationWarInst != null && m_myHero.nationId != nationWarInst.offenseNation.id)
			{
				Resource res = Resource.instance;
				m_myHero.SetPreviousContinentAndRotation(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationInst.nation);
				m_myHero.SetPreviousPositionAndRotation(res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation());
			}
		}
		MainQuestDungeonAbandonResponseBody resBody = new MainQuestDungeonAbandonResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
