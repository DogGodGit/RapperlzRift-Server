using ClientCommon;

namespace GameServer;

public class ContinentSaftyReviveCommandHandler : InGameCommandHandler<ContinentSaftyReviveCommandBody, ContinentSaftyReviveResponseBody>
{
	public const short kResult_NotDead = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태가 아닙니다.");
		}
		Continent targetContinent = null;
		Vector3 targetPosition = Vector3.zero;
		float fTargetRotatinoY = 0f;
		Resource res = Resource.instance;
		int nStartContinentId = res.startContinentId;
		if (currentPlace.continent.id == nStartContinentId && currentPlace.nationId == m_myHero.nationId)
		{
			targetContinent = res.GetContinent(res.startContinentId);
			if (targetContinent == null)
			{
				throw new CommandHandleException(1, "해당 대륙이 존재하지 않습니다. startContinentId = " + nStartContinentId);
			}
			targetPosition = res.SelectStartContinentSaftyRevivalPosition();
			fTargetRotatinoY = res.SelectStartContinentSaftyRevivalYRotation();
		}
		else
		{
			int nSaftyRevivalContinentId = res.saftyRevivalContinentId;
			targetContinent = res.GetContinent(nSaftyRevivalContinentId);
			if (targetContinent == null)
			{
				throw new CommandHandleException(1, "해당 대륙이 존재하지 않습니다. nSaftyRevivalContinentId = " + nSaftyRevivalContinentId);
			}
			targetPosition = res.SelectSaftyRevivalPosition();
			fTargetRotatinoY = res.SelectSaftyRevivalYRotation();
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForSaftyRevivalParam(targetContinent, m_myHero.nationId, targetPosition, fTargetRotatinoY));
		ContinentSaftyReviveResponseBody resBody = new ContinentSaftyReviveResponseBody();
		resBody.revivalTargetContinentId = targetContinent.id;
		resBody.revivalTargetNationId = m_myHero.nationId;
		SendResponseOK(resBody);
	}
}
