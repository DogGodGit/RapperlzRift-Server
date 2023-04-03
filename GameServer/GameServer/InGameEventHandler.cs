using System;
using System.Text;
using ClientCommon;

namespace GameServer;

public abstract class InGameEventHandler<TBody> : LoginRequiredEventHandler<TBody> where TBody : CEBClientEventBody
{
	protected Hero m_myHero;

	public Hero hero => m_myHero;

	protected override void HandleLoginRequiredEvent()
	{
		Hero myHero = m_myAccount.currentHero;
		if (myHero == null || !myHero.isLoggedIn)
		{
			throw new EventHandleException("영웅 로그인하지 않았습니다.", null, bLoggingEnabled: false);
		}
		m_myHero = myHero;
		HandleInGameEvent();
	}

	protected abstract void HandleInGameEvent();

	protected override void WriteLogFooter(StringBuilder sb)
	{
		base.WriteLogFooter(sb);
		if (sb.Length > 0)
		{
			sb.Append(Environment.NewLine);
		}
		sb.Append("#F ");
		sb.Append("HeroId : ");
		if (m_myHero == null)
		{
			sb.Append("null");
		}
		else
		{
			sb.Append(m_myHero.id);
		}
	}
}
