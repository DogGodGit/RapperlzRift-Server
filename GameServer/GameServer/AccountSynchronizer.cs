using System;
using ServerFramework;

namespace GameServer;

public class AccountSynchronizer : Synchronizer
{
	private Account m_account;

	protected override object syncObject => m_account.syncObject;

	protected override Hero hero => m_account.currentHero;

	public AccountSynchronizer(Account account, ISFRunnable runnable)
		: base(runnable)
	{
		if (account == null)
		{
			throw new ArgumentNullException("account");
		}
		m_account = account;
	}

	public static void Exec(Account account, ISFRunnable runnable)
	{
		AccountSynchronizer inst = new AccountSynchronizer(account, runnable);
		inst.Run();
	}
}
