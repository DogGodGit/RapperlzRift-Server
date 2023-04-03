using System;
using ClientCommon;

namespace GameServer;

public class MonsterMoveEventHandler : InGameEventHandler<CEBMonsterMoveEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			if (m_body == null)
			{
				throw new EventHandleException("바디가 null입니다.");
			}
			long lnInstanceId = m_body.instanceId;
			Vector3 position = m_body.position;
			float fRotationY = m_body.rotationY;
			if (!currentPlace.Contains(position))
			{
				throw new EventHandleException("위치가 유효하지 않습니다. position = " + position);
			}
			MonsterInstance monsterInst = currentPlace.GetMonster(lnInstanceId);
			if (monsterInst != null && monsterInst.owner == m_myHero && monsterInst.moveEnabled)
			{
				DateTimeOffset currentTime = DateTimeUtil.currentTime;
				currentPlace.MoveMonster(monsterInst, position, fRotationY, currentTime);
				monsterInst.UpdateReturnMode();
			}
		}
	}
}
