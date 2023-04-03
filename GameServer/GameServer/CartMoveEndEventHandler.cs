using System;
using ClientCommon;

namespace GameServer;

public class CartMoveEndEventHandler : InGameEventHandler<CEBCartMoveEndEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_body == null)
		{
			throw new EventHandleException("바디가 null입니다.");
		}
		Guid placeInstanceId = (Guid)m_body.placeInstanceId;
		if (placeInstanceId == Guid.Empty)
		{
			throw new EventHandleException("장소ID가 유효하지 않습니다.");
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			return;
		}
		if (placeInstanceId != currentPlace.instanceId)
		{
			throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
		}
		CartInstance cartInst = m_myHero.ridingCartInst;
		if (cartInst == null)
		{
			throw new EventHandleException("현재 카트를 타고있지 않습니다.");
		}
		lock (cartInst.syncObject)
		{
			cartInst.EndMove();
		}
	}
}
