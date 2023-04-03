using System;
using ClientCommon;

namespace GameServer;

public class MoveModeChangedEventHandler : InGameEventHandler<CEBMoveModeChangedEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_body == null)
		{
			throw new EventHandleException("body가 null입니다.");
		}
		bool bIsWalking = m_body.isWalking;
		bool bIsManualMoving = m_body.isManualMoving;
		if (!m_myHero.moving)
		{
			throw new EventHandleException("현재 영웅이 움직이는 중이 아닙니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (bIsWalking)
		{
			m_myHero.StartWalking();
		}
		else
		{
			m_myHero.StartRunning(currentTime);
		}
		if (bIsManualMoving)
		{
			m_myHero.StartManualMoving(currentTime);
		}
		else
		{
			m_myHero.StopManualMoving();
		}
	}
}
