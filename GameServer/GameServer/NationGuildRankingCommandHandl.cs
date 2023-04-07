using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class NationGuildRankingCommandHandler : InGameCommandHandler<NationGuildRankingCommandBody, NationGuildRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		GuildRanking myGuildRanking = null;
		if (m_myHero.guildMember != null)
		{
			myGuildRanking = nationInst.GetGuildRankingOfGuild(m_myHero.guildMember.guild.id);
		}
		NationGuildRankingResponseBody resBody = new NationGuildRankingResponseBody();
		resBody.myGuildRanking = myGuildRanking?.ToPDGuildRanking();
		resBody.guildRankings = nationInst.GetPDGuildRankings(Resource.instance.guildRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
