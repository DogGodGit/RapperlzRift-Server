using ClientCommon;

namespace GameServer;

public class GuildListCommandHandler : InGameCommandHandler<GuildListCommandBody, GuildListResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		GuildListResponseBody resBody = new GuildListResponseBody();
		resBody.guilds = nationInst.GetPDSimpleGuilds().ToArray();
		SendResponseOK(resBody);
	}
}
