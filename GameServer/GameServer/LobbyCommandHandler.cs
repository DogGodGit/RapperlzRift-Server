using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class LobbyCommandHandler<TCommandBody, TResponseBody> : LoginRequiredCommandHandler<TCommandBody, TResponseBody> where TCommandBody : CommandBody where TResponseBody : ResponseBody
{
	protected override void HandleLoginRequiredCommand()
	{
		if (m_myAccount.currentHero != null)
		{
			throw new CommandHandleException(1, string.Concat("현재 로비가 아닙니다. heroId = ", m_myAccount.currentHero.id, ", heroName = ", m_myAccount.currentHero.name));
		}
		HandleLobbyCommand();
	}

	protected abstract void HandleLobbyCommand();

	protected override void OnReentry(ISFRunnable work)
	{
		base.OnReentry(work);
		if (this is HeroLoginCommandHandler || m_myAccount.currentHero == null)
		{
			return;
		}
		throw new CommandHandleException(1, string.Concat("[재진입] 현재 로비가 아닙니다. heroId = ", m_myAccount.currentHero.id, ", heroName = ", m_myAccount.currentHero.name));
	}
}
