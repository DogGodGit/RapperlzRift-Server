using System;
using System.Text;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class InGameCommandHandler<TCommandBody, TResponseBody> : LoginRequiredCommandHandler<TCommandBody, TResponseBody> where TCommandBody : CommandBody where TResponseBody : ResponseBody
{
	protected Hero m_myHero;

	public Hero hero => m_myHero;

	protected override void HandleLoginRequiredCommand()
	{
		Hero myHero = m_myAccount.currentHero;
		if (myHero == null || !myHero.isLoggedIn)
		{
			throw new CommandHandleException(1, "영웅 로그인하지 않았습니다.", null, bLoggingEnabled: false);
		}
		m_myHero = myHero;
		HandleInGameCommand();
	}

	protected abstract void HandleInGameCommand();

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

	protected override void OnReentry(ISFRunnable work)
	{
		base.OnReentry(work);
		if (!m_myHero.isLoggedIn)
		{
			throw new CommandHandleException(1, "[재진입] 영웅이 로그인상태가 아닙니다.");
		}
	}
}
