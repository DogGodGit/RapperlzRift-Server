using ClientCommon;

namespace GameServer;

public class NationWarInfoCommandHandler : InGameCommandHandler<NationWarInfoCommandBody, NationWarInfoResponseBody>
{
	public const short kResult_NoNationWar = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationInst.nationWarInst == null)
		{
			throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
		}
		NationWarInfoResponseBody resBody = new NationWarInfoResponseBody();
		resBody.monsterInsts = nationWarInst.GetPDSimpleNationWarMonsterInstances().ToArray();
		SendResponseOK(resBody);
	}
}
