using System;
using ClientCommon;

namespace GameServer;

public class CartAccelerateCommandHandler : InGameCommandHandler<CartAccelerateCommandBody, CartAccelerateResponseBody>
{
	public const short kResult_NotCartRiding = 101;

	public const short kResult_CoolTimeNotElapsed = 102;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		CartInstance cartInst = m_myHero.ridingCartInst;
		if (cartInst == null)
		{
			throw new CommandHandleException(101, "카트를 타고있지 않습니다.");
		}
		lock (cartInst.syncObject)
		{
			if (!cartInst.IsAccelCoolTimeElapsed(currentTime))
			{
				throw new CommandHandleException(102, "쿨타임이 경과되지 않았습니다.");
			}
			bool isSuccess = cartInst.Accelerate(currentTime);
			CartAccelerateResponseBody resBody = new CartAccelerateResponseBody();
			resBody.isSuccess = isSuccess;
			resBody.remainingAccelCoolTime = cartInst.GetRemainingAccelCoolTime(currentTime);
			SendResponseOK(resBody);
		}
	}
}
