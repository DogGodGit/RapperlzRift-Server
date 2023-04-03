using System;
using System.Text;
using ClientCommon;

namespace GameServer;

public abstract class LoginRequiredEventHandler<TBody> : EventHandler<TBody> where TBody : CEBClientEventBody
{
	protected Account m_myAccount;

	public Account account => m_myAccount;

	protected override void HandleEventInternal()
	{
		Account account = base.clientPeer.account;
		if (account == null || !account.isLoggedIn)
		{
			throw new EventHandleException("계정 로그인하지 않았습니다.", null, bLoggingEnabled: false);
		}
		m_myAccount = account;
		HandleLoginRequiredEvent();
	}

	protected abstract void HandleLoginRequiredEvent();

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
}
