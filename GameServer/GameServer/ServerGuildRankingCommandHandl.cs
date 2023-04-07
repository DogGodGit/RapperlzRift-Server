using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ServerGuildRankingCommandHandler : InGameCommandHandler<ServerGuildRankingCommandBody, ServerGuildRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerGuildRankingManager serverGuildRankingManager = ServerGuildRankingManager.instance;
		GuildRanking myGuildRanking = null;
		if (m_myHero.guildMember != null)
		{
			myGuildRanking = serverGuildRankingManager.GetGuildRankingOfGuild(m_myHero.guildMember.guild.id);
		}
		ServerGuildRankingResponseBody resBody = new ServerGuildRankingResponseBody();
		resBody.myGuildRanking = myGuildRanking?.ToPDGuildRanking();
		resBody.guildRankings = serverGuildRankingManager.GetPDRankings(Resource.instance.guildRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
