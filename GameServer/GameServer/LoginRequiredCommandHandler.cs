using System;
using System.Text;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class LoginRequiredCommandHandler<TCommandBody, TResponseBody> : CommandHandler<TCommandBody, TResponseBody> where TCommandBody : CommandBody where TResponseBody : ResponseBody
{
	protected Account m_myAccount;

	public Account account => m_myAccount;

	protected override void HandleCommandInternal()
	{
		Account account = base.clientPeer.account;
		if (account == null || !account.isLoggedIn)
		{
			throw new CommandHandleException(1, "계정 로그인하지 않았습니다.", null, bLoggingEnabled: false);
		}
		m_myAccount = account;
		HandleLoginRequiredCommand();
	}

	protected abstract void HandleLoginRequiredCommand();

	protected override void WriteLogFooter(StringBuilder sb)
	{
		base.WriteLogFooter(sb);
		if (sb.Length > 0)
		{
			sb.Append(Environment.NewLine);
		}
		sb.Append("#F ");
		sb.Append("AccountId : ");
		if (m_myAccount == null)
		{
			sb.Append("null");
		}
		else
		{
			sb.Append(m_myAccount.id);
		}
	}

	protected override void OnReentry(ISFRunnable work)
	{
		base.OnReentry(work);
		if (!m_myAccount.isLoggedIn)
		{
			throw new CommandHandleException(1, "[재진입] 계정이 로그인상태가 아닙니다.");
		}
	}
}
